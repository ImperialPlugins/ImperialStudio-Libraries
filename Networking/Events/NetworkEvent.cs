
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Networking.Events
{
    public class NetworkEvent : Event, ICancellableEvent
    {
        public ENet.Event EnetEvent { get; }
        public NetworkPeer NetworkPeer { get; }

        public NetworkEvent(ENet.Event enetEvent, NetworkPeer networkPeer)
        {
            EnetEvent = enetEvent;
            NetworkPeer = networkPeer;
        }

        public bool IsCancelled { get; set; }
    }
}