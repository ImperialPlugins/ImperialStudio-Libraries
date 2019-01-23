
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Networking.Events
{
    public class NetworkEvent : Event, ICancellableEvent
    {
        public ENet.Event EnetEvent { get; }

        public NetworkEvent(ENet.Event enetEvent)
        {
            EnetEvent = enetEvent;
        }

        public bool IsCancelled { get; set; }
    }
}