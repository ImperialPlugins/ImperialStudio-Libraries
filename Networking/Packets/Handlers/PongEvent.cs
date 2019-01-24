using Event = ImperialStudio.Core.Eventing.Event;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    public sealed class PongEvent : Event
    {
        public PongEvent(NetworkPeer peer, PongPacket pongPacket)
        {
            Peer = peer;
            PongPacket = pongPacket;
        }

        public NetworkPeer Peer { get; }
        public PongPacket PongPacket { get; }
    }
}