using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Logging;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Reflection;
using ImperialStudio.Extensions.Reflection;

namespace ImperialStudio.Core.Eventing
{
    public class EventBus : IEventBus
    {
        private readonly IWindsorContainer container;
        private readonly List<EventAction> _eventListeners = new List<EventAction>();

        private readonly List<IEvent> _inProgress = new List<IEvent>();
        private readonly ILogger logger;

        public EventBus(IWindsorContainer container, ILogger logger)
        {
            this.container = container;
            this.logger = logger;
        }

        public void Subscribe(object @object, string eventName, EventCallback callback)
        {
            EventHandler handler = GetEventHandler(callback.Method);
            _eventListeners.Add(new EventAction(@object, callback.Invoke, handler, new List<string> { eventName }));
        }

        public void Subscribe<TEvent>(object @object, EventCallback<TEvent> callback) where TEvent : IEvent
        {
            Subscribe(@object, typeof(TEvent), (s, e) => callback(s, (TEvent) e));
        }

        public void Subscribe(object @object, Type eventType, EventCallback callback)
        {
            EventHandler handler = GetEventHandler(callback.Method);
            _eventListeners.Add(new EventAction(@object, callback.Invoke, handler, eventType));
        }

        public void Unsubscribe(object @object)
        {
            _eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void Unsubscribe<TEvent>(object @object) where TEvent : IEvent
        {
            _eventListeners.RemoveAll(c => c.Owner == @object);
        }

        public void Unsubscribe(object @object, Type eventType)
        {
            _eventListeners.RemoveAll(c => c.Owner == @object && CheckEvent(c, GetEventNames(eventType)));
        }

        public void AddEventListener(object @object, IEventListener eventListener)
        {
            // ReSharper disable once UseIsOperator.2
            if (!typeof(IEventListener).IsInstanceOfType(eventListener))
                throw new ArgumentException(
                    "The eventListener to register has to implement IEventListener!",
                    nameof(eventListener));

            if (_eventListeners.Any(c => c.Listener?.GetType() == eventListener.GetType())) return;

            Type type = eventListener.GetType();

            foreach (Type @interface in type.GetInterfaces()
                                            .Where(c => typeof(IEventListener).IsAssignableFrom(c)
                                                && c.GetGenericArguments().Length > 0))
                foreach (MethodInfo method in @interface.GetMethods())
                {
                    EventHandler handler = (EventHandler)method.GetCustomAttributes(typeof(EventHandler), false)
                                                                .FirstOrDefault()
                        ?? new EventHandler
                        {
                            Priority = EventPriority.Normal
                        };

                    Type eventType = @interface.GetGenericArguments()[0];

                    _eventListeners.Add(new EventAction(@object, eventListener, method, handler, eventType));
                }
        }

        public void RemoveEventListener(IEventListener eventListener)
        {
            _eventListeners.RemoveAll(c => c.Listener == eventListener);
        }

        public void Emit(object sender, IEvent @event, EventExecutedCallback callback = null)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

#if LOG_EVENTS
            string eventNameString = $"{string.Join(", ", @event.Names.Select(d => d + "Event"))}";
            logger.LogDebug(eventNameString + " fired.");
#endif

            _inProgress.Add(@event);

            var names = GetEventNames(@event.GetType());

            List<EventAction> actions =
                _eventListeners
                    .Where(c => c.TargetEventType?.IsInstanceOfType(@event)
                        ?? names.Any(d => c.TargetEventNames.Any(e => d.Equals(e, StringComparison.OrdinalIgnoreCase))))
                    .ToList();

            actions.Sort((a, b) => EventPriorityComparer.Compare(a.Handler.Priority, b.Handler.Priority));

            List<EventAction> targetActions =
                (from info in actions
                     /* ignore cancelled events */
                 where !(@event is ICancellableEvent)
                     || !((ICancellableEvent)@event).IsCancelled
                     || info.Handler.IgnoreCancelled
                 where CheckEvent(info, names)
                 select info)
                .ToList();

            void FinishEvent()
            {
                _inProgress.Remove(@event);
                callback?.Invoke(@event);
            }

            if (targetActions.Count == 0)
            {
#if LOG_EVENTS
                logger.LogDebug(eventNameString + ": No listeners found.");
#endif
                FinishEvent();
                return;
            }

            foreach (EventAction info in targetActions)
            {
                var wk = info.Owner;
                
                //check if owner is alive
                if (!wk.TryGetTarget(out object _))
                {
                    actions.RemoveAll(c => c.Owner == wk);
                }

                info.Action.Invoke(sender, @event);
            }

            FinishEvent();
        }

        public bool HasFinished(IEvent @event) => !_inProgress.Contains(@event);

        private EventHandler GetEventHandler(Type target)
        {
            EventHandler handler =
                (EventHandler)target?.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? new EventHandler();
            return handler;
        }

        private EventHandler GetEventHandler(MethodInfo target)
        {
            EventHandler handler =
                (EventHandler)target.GetCustomAttributes(typeof(EventHandler), false).FirstOrDefault()
                ?? GetEventHandler(target.DeclaringType);
            return handler;
        }

        private bool CheckEvent(EventAction eventAction, IReadOnlyCollection<string> eventNames)
        {
            if (eventAction.TargetEventType == null)
            {
                return eventNames.Any(c => eventAction.TargetEventNames.Any(e => c.Equals(e, StringComparison.OrdinalIgnoreCase)));
            }

            return eventNames.Any(c => GetEventNames(eventAction.TargetEventType).Any(d => d.Equals(c, StringComparison.OrdinalIgnoreCase)));
        }

        private static readonly Dictionary<Type, string[]> m_EventNamesCache = new Dictionary<Type, string[]>();
        public static IReadOnlyCollection<string> GetEventNames(Type t)
        {
            if (!m_EventNamesCache.ContainsKey(t))
            {
                List<string> names = new List<string>();
                foreach (var type in t.GetTypeHierarchy())
                {
                    if (!typeof(IEvent).IsAssignableFrom(type))
                        break;

                    var attr = type.GetCustomAttributes(typeof(EventNameAttribute), false)
                        .Cast<EventNameAttribute>()
                        .ToList();
                    if (attr.Count == 0)
                    {
                        names.Add(type.Name.Replace("Event", ""));
                        continue;
                    }

                    names.AddRange(attr.Select(c => c.EventName));
                }

                m_EventNamesCache.Add(t, names.ToArray());
            }

            return m_EventNamesCache[t];
        }
    }
}