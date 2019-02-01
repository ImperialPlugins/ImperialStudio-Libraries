using Castle.Windsor;
using ImperialStudio.Core.Api.Entities;
using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Scheduling;
using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using ILogger = ImperialStudio.Core.Api.Logging.ILogger;

namespace ImperialStudio.Core.Entities
{
    public class EntityManager : IEntityManager
    {
        private readonly IEventBus m_EventBus;
        private readonly ILogger m_Logger;
        private readonly ITaskScheduler m_TaskScheduler;
        private readonly IWindsorContainer m_Container;
        private readonly Dictionary<int, IEntity> m_SpawnedEntities;
        private int m_NextId;

        public EntityManager(
            IEventBus eventBus,
            ILogger logger,
            ITaskScheduler taskScheduler,
            IWindsorContainer container)
        {
            m_EventBus = eventBus;
            m_Logger = logger;
            m_TaskScheduler = taskScheduler;
            m_Container = container;
            m_SpawnedEntities = new Dictionary<int, IEntity>();
        }

        public void DespawnEntities(INetworkPeer owner, Action callback)
        {
            m_TaskScheduler.ScheduleUpdate(this, () =>
                {
                    var allEntities = GetEntities().ToList();
                    foreach (var ent in allEntities.Where(d => d.Owner.Id == owner.Id))
                    {
                        Despawn(ent);
                    }

                    callback();
                }, $"DespawnEntities-[{owner.Username}]", ExecutionTargetContext.NextFrame);
        }

        public IEnumerable<IEntity> GetEntities()
        {
            lock (m_SpawnedEntities)
            {
                return m_SpawnedEntities.Values;
            }
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

        public IEntity GetEntitiy(int id)
        {
            lock (m_SpawnedEntities)
            {
                if (!m_SpawnedEntities.ContainsKey(id))
                {
                    throw new Exception($"Entity #{id} was not found.");
                }

                return m_SpawnedEntities[id];
            }
        }

        public void Spawn(int id, Type type, bool isOwner, Action<IEntity> callback = null)
        {
            lock (m_SpawnedEntities)
            {
                if (m_SpawnedEntities.ContainsKey(id))
                {
                    throw new Exception($"Entity #{id} exists already!");
                }
            }

            if (!typeof(IEntity).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface || !type.IsClass)
            {
                throw new Exception("Failed to spawn invalid entity: " + type.FullName);
            }

            m_Logger.LogDebug($"Spawning entity {type.Name}@{id}");

            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                var entityInstance = (IEntity)m_Container.Activate(type);
                entityInstance.Id = id;
                entityInstance.IsOwner = isOwner;
                entityInstance.Init();

                lock (m_SpawnedEntities)
                {
                    m_SpawnedEntities.Add(entityInstance.Id, entityInstance);
                }

                callback?.Invoke(entityInstance);
            }, $"SpawnEntity-[{type.Name}]@{id}");
        }

        public void Spawn<TEntity>(INetworkPeer owner, Action<TEntity> callback = null) where TEntity : class, IEntity
        {
            int idToUse = Interlocked.Increment(ref m_NextId);

            var @event = new EntitySpawnEvent(idToUse, typeof(TEntity));
            m_EventBus.Emit(this, @event);

            if (@event.IsCancelled)
            {
                Interlocked.Decrement(ref m_NextId);
                return;
            }

            m_TaskScheduler.RunOnMainThread(this, () =>
                    {

                        var entityInstance = (TEntity)m_Container.Activate(typeof(TEntity));
                        entityInstance.Id = idToUse;
                        entityInstance.Owner = owner;
                        entityInstance.Init();
                        lock (m_SpawnedEntities)
                        {
                            m_SpawnedEntities.Add(entityInstance.Id, entityInstance);
                        }

                        callback?.Invoke(entityInstance);
                    }, $"SpawnEntity-[{typeof(TEntity).Name}]");
        }

        public void Despawn(int id, Action callback = null)
        {

            var entity = GetEntitiy(id);
            if (entity.IsDisposed)
            {
                lock (m_SpawnedEntities)
                {
                    m_SpawnedEntities.Remove(id);
                }

                return;
            }

            var @event = new EntityDespawnEvent(entity);
            m_EventBus.Emit(this, @event);

            if (@event.IsCancelled)
            {
                return;
            }

            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                entity.Dispose();
                lock (m_SpawnedEntities)
                {
                    m_SpawnedEntities.Remove(id);
                }

                callback?.Invoke();
            }, $"Despawn-{id}");
        }

        public void Despawn(IEntity entity, Action callback = null)
        {
            Despawn(entity.Id, callback);
        }
    }
}