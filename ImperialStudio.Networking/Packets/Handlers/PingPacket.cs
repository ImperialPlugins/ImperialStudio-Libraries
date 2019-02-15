using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Ping)]
    [MessagePackObject]
    public class PingPacket : IPacket
    {
        [Key(0)]
        public virtual ulong PingId { get; set; }
    }
}