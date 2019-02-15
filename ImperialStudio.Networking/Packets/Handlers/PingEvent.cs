using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking.Packets.Handlers
{
    public sealed class PingEvent : IEvent, ICancellableEvent
    {
        public PingEvent(INetworkPeer sender, PingPacket pingPacket)
        {
            Peer = sender;
            PingPacket = pingPacket;
        }

        public INetworkPeer Peer { get; }
        public PingPacket PingPacket { get; }
        public bool IsCancelled { get; set; }
    }
}