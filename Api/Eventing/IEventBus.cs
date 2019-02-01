using System;

namespace ImperialStudio.Core.Api.Eventing
{
    /// <summary>
    ///     The type safe callback for event notifications.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="sender">The event sender.</param>
    /// <param name="event">The event instance.</param>
    public delegate void EventCallback<in TEvent>(object sender, TEvent @event) where TEvent : IEvent;

    /// <summary>
    ///     The callback for event notifications.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="event">The event instance.</param>
    public delegate void EventCallback(object sender, IEvent @event);

    /// <summary>
    ///     The emit callback for events that have finished and notified all listeners.
    /// </summary>
    /// <param name="event"></param>
    public delegate void EventExecutedCallback(IEvent @event);

    /// <summary>
    ///     The event manager is responsible for emitting events and for managing their subscriptions.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        ///     Subscribe to an event.
        /// </summary>
        /// <param name="object">The associated object.</param>
        /// <param name="eventName">The event to subscribe to.</param>
        /// <param name="callback">The action to execute. See <see cref="EventCallback" /></param>
        void Subscribe(object o, string eventName, EventCallback callback);

        /// <summary>
        ///     <inheritdoc cref="Subscribe(object, string, EventCallback)" />
        /// </summary>
        /// <param name="object">The associated object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        void Subscribe<TEvent>(object @object, EventCallback<TEvent> callback) where TEvent: IEvent;

        /// <summary>
        ///     <inheritdoc cref="Subscribe(object ,string, EventCallback)" />
        /// </summary>
        /// <param name="object">The associated object.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <param name="eventType">The event to subscribe to.</param>
        void Subscribe(object @object, Type eventType, EventCallback callback);


        /// <summary>
        /// Checks if an event has finished.
        /// </summary>
        /// <returns></returns>
        bool HasFinished(IEvent @event);

        /// <summary>
        ///     Unsubscribe all listener subscriptions of the given object.
        /// </summary>
        /// <param name="object">The associated object.</param>
        void Unsubscribe(object @object);

        /// <summary>
        ///     Unsubscribe the event from this object type-safe
        /// </summary>
        /// <param name="object">The associated object.</param>
        void Unsubscribe<TEvent>(object @object) where TEvent : IEvent;

        /// <summary>
        ///     Unsubscribe all subscriptions for the given event type of the given object.
        /// </summary>
        /// <param name="object">The associated object.</param>
        /// <param name="eventType">The event to unsubscribe from.</param>
        void Unsubscribe(object @object, Type eventType);

        /// <summary>
        ///     Register an event listener instance.
        /// </summary>
        /// <param name="object">The associated object.</param>
        /// <param name="eventListener">The event listener to register.</param>
        void AddEventListener(object @object, IEventListener eventListener);

        /// <summary>
        ///     Remove an event listeners subscription.
        /// </summary>
        /// <param name="eventListener">The event listener to remove.</param>
        void RemoveEventListener(IEventListener eventListener);

        /// <summary>
        ///     Emits an event and optionally handles the result
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="event">The event instance.</param>
        /// <param name="callback">The event finish callback. See <see cref="EventExecutedCallback" />.</param>
        void Emit(object sender, IEvent @event, EventExecutedCallback callback = null);
    }
}