using ENet;

namespace ImperialStudio.Core.Networking.Packets
{
    public struct IncomingPacket
    {
        public Peer Peer { get; set; }
        public PacketType PacketType { get; set; }
        public byte[] Data { get; set; }
        public bool IsVerified { get; set; }
        public byte ChannelId { get; set; }
    }
}