using System;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Entities
{
    public class EntitySpawnEvent : Event, ICancellableEvent
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