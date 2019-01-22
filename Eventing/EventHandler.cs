using System;

namespace ImperialStudio.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute
    {
        public EventPriority Priority { get; set; } = EventPriority.Normal;

        public bool IgnoreCancelled { get; set; } = false;
    }
}