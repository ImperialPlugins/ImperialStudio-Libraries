using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Pong)]
    [MessagePackObject]
    public class PongPacket : IPacket
    {
        [Key(0)]
        public virtual ulong PingId { get; set; }
    }
}