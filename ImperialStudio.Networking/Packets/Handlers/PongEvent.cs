using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking.Packets.Handlers
{
    public sealed class PongEvent : IEvent
    {
        public PongEvent(INetworkPeer peer, PongPacket pongPacket)
        {
            Peer = peer;
            PongPacket = pongPacket;
        }

        public INetworkPeer Peer { get; }
        public PongPacket PongPacket { get; }
    }
}