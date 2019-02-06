using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Entities
{
    public class EntityDespawnEvent : Event, ICancellableEvent
    {
        public IEntity Entity { get; }

        public EntityDespawnEvent(IEntity entity)
        {
            Entity = entity;
        }

        public bool IsCancelled { get; set; }
    }
}