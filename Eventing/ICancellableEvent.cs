namespace ImperialStudio.Core.Eventing
{
    /// <summary>
    ///     Defines an event that can be cancelled.
    /// </summary>
    public interface ICancellableEvent
    {
        /// <summary>
        ///     Defines if the event action should be cancelled.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}