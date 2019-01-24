namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Pong)]
    public class PongPacket : IPacket
    {
        public ulong PingId { get; set; }
    }
}