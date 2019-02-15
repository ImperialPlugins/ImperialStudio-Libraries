using ImperialStudio.Api.Eventing;

namespace ImperialStudio.Api.Entities
{
    public class EntityDespawnEvent : IEvent, ICancellableEvent
    {
        public IEntity Entity { get; }

        public EntityDespawnEvent(IEntity entity)
        {
            Entity = entity;
        }

        public bool IsCancelled { get; set; }
    }
}