using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Networking;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Networking.Events
{
    public class NetworkEvent : Event, ICancellableEvent
    {
        public ENet.Event EnetEvent { get; }
        public INetworkPeer NetworkPeer { get; }

        public NetworkEvent(ENet.Event enetEvent, INetworkPeer networkPeer)
        {
            EnetEvent = enetEvent;
            NetworkPeer = networkPeer;
        }

        public bool IsCancelled { get; set; }
    }
}