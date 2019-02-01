using System;
using System.Collections.Generic;
using System.Reflection;
using ImperialStudio.Core.Api.Eventing;

namespace ImperialStudio.Core.Eventing
{
    internal class EventAction
    {
        public EventAction(
            object owner,
            EventCallback action,
            EventHandler handler,
            List<string> eventNames)
        {
            Owner = new WeakReference<object>(owner);
            Action = action;
            Handler = handler;
            TargetEventNames = eventNames;
        }

        public EventAction(object owner,
            IEventListener listener,
            MethodInfo method,
            EventHandler handler, Type type)
        {
            Owner = new WeakReference<object>(owner);
            Listener = listener;
            Action = (sender, @event) => method.Invoke(listener, new[] { sender, @event });
            Handler = handler;
            TargetEventNames = EventBus.GetEventNames(type);
            TargetEventType = type;
        }

        public EventAction(object owner, EventCallback action, EventHandler handler, Type eventType)
        {
            Owner = new WeakReference<object>(owner);
            Action = action;
            Handler = handler;
            TargetEventNames = EventBus.GetEventNames(eventType);
            TargetEventType = eventType;
        }

        public Type TargetEventType { get; set; }

        public WeakReference<object> Owner { get; set; }

        public EventCallback Action { get; }

        public EventHandler Handler { get; }

        public IEventListener Listener { get; }

        public List<string> TargetEventNames { get; }
    }
}