using System;
using System.Collections.Generic;
using System.Numerics;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Api.Entities
{
    public interface IEntityManager
    {
        void Spawn(int id, Type type, bool isOwner, Action<IEntity> callback = null);

        void Spawn<TEntity>(INetworkPeer owner, Action<TEntity> callback = null) where TEntity : class, IEntity;

        void Despawn(int id, Action callback = null);

        void Despawn(IEntity entity, Action callback = null);

        IEnumerable<IEntity> GetEntities();

        IEnumerable<IEntity> GetEntities(Vector3 relativeTo, float radius);

        IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : class, IEntity;

        IEnumerable<TEntity> GetEntities<TEntity>(Vector3 relativeTo, float radius) where TEntity : class, IEntity;
        
        IEntity GetEntitiy(int id);

        void DespawnEntities(INetworkPeer owner, Action callback = null);
    }
}