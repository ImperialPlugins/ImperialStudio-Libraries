using ImperialStudio.Core.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using ImperialStudio.Core.DependencyInjection;
using UnityEngine;

namespace ImperialStudio.Core.Entities
{
    public class EntityManager : IEntityManager
    {
        private readonly IEventBus m_EventBus;
        private readonly IWindsorContainer m_Container;
        private readonly Dictionary<ushort, IEntity> m_SpawnedEntities;
        private ushort m_NextId;

        public EntityManager(IEventBus eventBus, IWindsorContainer container)
        {
            m_EventBus = eventBus;
            m_Container = container;
            m_SpawnedEntities = new Dictionary<ushort, IEntity>();
        }

        public IEnumerable<IEntity> GetEntities()
        {
            return m_SpawnedEntities.Values;
        }

        public IEnumerable<IEntity> GetEntities(Vector3 relativeTo, float radius)
        {
            return GetEntities().Where(d => d.Transform != null && Vector3.Distance(relativeTo, d.Transform.position) <= radius);
        }

        public IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : class, IEntity
        {
            return GetEntities().Where(c => c is TEntity).Cast<TEntity>();
        }

        public IEnumerable<TEntity> GetEntities<TEntity>(Vector3 relativeTo, float radius) where TEntity : class, IEntity
        {
            return GetEntities<TEntity>().Where(d => d.Transform != null && Vector3.Distance(relativeTo, d.Transform.position) <= radius);
        }

        public IEntity GetEntitiy(ushort id)
        {
            if (!m_SpawnedEntities.ContainsKey(id))
            {
                throw new Exception($"Entity #{id} was not found.");
            }

            return m_SpawnedEntities[id];
        }

        public IEntity Spawn(ushort id, Type type)
        {
            if (m_SpawnedEntities.ContainsKey(id))
            {
                throw new Exception($"Entity #{id} exists already!");
            }

            if (!typeof(IEntity).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface || !type.IsClass)
            {
                throw new Exception("Failed to spawn invalid entity: " + type.FullName);
            }

            var entityInstance = (IEntity) m_Container.Activate(type);
            entityInstance.Id = id;
            entityInstance.Init();

            m_SpawnedEntities.Add(entityInstance.Id, entityInstance);
            return entityInstance;
        }

        public TEntity Spawn<TEntity>() where TEntity : class, IEntity
        {
            lock (m_SpawnedEntities)
            {
                ushort idToUse = m_NextId++;

                var @event = new EntitySpawnEvent(idToUse, typeof(TEntity));
                m_EventBus.Emit(this, @event);

                if (@event.IsCancelled)
                {
                    m_NextId--;
                    return null;
                }

                var entityInstance = (TEntity)m_Container.Activate(typeof(TEntity));
                entityInstance.Id = idToUse;
                entityInstance.Init();

                m_SpawnedEntities.Add(entityInstance.Id, entityInstance);
                return entityInstance;
            }
        }

        public void Despawn(ushort id)
        {
            lock (m_SpawnedEntities)
            {
                var entity = GetEntitiy(id);
                if (entity.IsDisposed)
                {
                    m_SpawnedEntities.Remove(id);
                    return;
                }

                var @event = new EntityDespawnEvent(entity);
                m_EventBus.Emit(this, @event);

                if (@event.IsCancelled)
                {
                    return;
                }

                entity.Dispose();
                m_SpawnedEntities.Remove(id);
            }
        }

        public void Despawn(IEntity entity)
        {
            Despawn(entity.Id);
        }
    }
}