using System;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Entities
{
    public class EntitySpawnEvent : Event, ICancellableEvent
    {
        public ushort EntityId { get; }
        public Type EntityType { get; }

        public EntitySpawnEvent(ushort entityId, Type type)
        {
            EntityId = entityId;
            EntityType = type;
        }

        public bool IsCancelled { get; set; }
    }
}