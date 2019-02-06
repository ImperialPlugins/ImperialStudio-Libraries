using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Networking;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Networking.Packets.Handlers
{
    public sealed class PingEvent : Event, ICancellableEvent
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