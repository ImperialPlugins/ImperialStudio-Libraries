using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImperialStudio.Core.Entities
{
    public interface IEntityManager
    {
        TEntity Spawn<TEntity>() where TEntity : class, IEntity;

        void Despawn(ushort id);

        void Despawn(IEntity entity);

        IEnumerable<IEntity> GetEntities();

        IEnumerable<IEntity> GetEntities(Vector3 relativeTo, float radius);

        IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : class, IEntity;

        IEnumerable<TEntity> GetEntities<TEntity>(Vector3 relativeTo, float radius) where TEntity : class, IEntity;
        
        IEntity GetEntitiy(ushort id);
        IEntity Spawn(ushort id, Type type);
    }
}