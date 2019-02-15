using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking.Events
{
    public class ENetNetworkEvent : IEvent, ICancellableEvent
    {
        public ENet.Event Event { get; }
        public INetworkPeer NetworkPeer { get; }

        public ENetNetworkEvent(ENet.Event @event, INetworkPeer networkPeer)
        {
            Event = @event;
            NetworkPeer = networkPeer;
        }

        public bool IsCancelled { get; set; }
    }
}