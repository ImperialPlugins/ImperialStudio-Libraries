using ENet;
using ImperialStudio.Core.Eventing;
using Event = ImperialStudio.Core.Eventing.Event;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    public sealed class PingEvent : Event, ICancellableEvent
    {
        public PingEvent(NetworkPeer sender, PingPacket pingPacket)
        {
            Peer = sender;
            PingPacket = pingPacket;
        }
        public NetworkPeer Peer { get; }
        public PingPacket PingPacket { get; }
        public bool IsCancelled { get; set; }
    }
}