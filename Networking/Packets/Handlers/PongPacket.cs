using ZeroFormatter;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Pong)]
    [ZeroFormattable]
    public class PongPacket : IPacket
    {
        [Index(0)]
        public virtual ulong PingId { get; set; }
    }
}