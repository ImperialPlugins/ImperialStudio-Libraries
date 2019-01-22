using System.Collections.Generic;

namespace ImperialStudio.Core.Eventing
{
    /// <summary>
    ///     Base representation of an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        ///     The name of the event with the name of the parent types.
        ///     For example: "playerconnected", "userconnected".
        ///     <b>Each event instance should only have one name.</b>
        /// </summary>
        IEnumerable<string> Names { get; }

        /// <summary>
        ///     The arguments of the event.
        /// </summary>
        Dictionary<string, object> Arguments { get; }
    }
}