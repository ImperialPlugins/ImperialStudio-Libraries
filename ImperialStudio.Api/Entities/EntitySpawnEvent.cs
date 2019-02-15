using System;
using ImperialStudio.Api.Eventing;

namespace ImperialStudio.Api.Entities
{
    public class EntitySpawnEvent : IEvent, ICancellableEvent
    {
        public int EntityId { get; }
        public Type EntityType { get; }

        public EntitySpawnEvent(int entityId, Type type)
        {
            EntityId = entityId;
            EntityType = type;
        }

        public bool IsCancelled { get; set; }
    }
}