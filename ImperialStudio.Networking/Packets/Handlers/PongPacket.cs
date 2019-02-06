using ImperialStudio.Api.Networking.Packets;
using ZeroFormatter;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Pong)]
    [ZeroFormattable]
    public class PongPacket : IPacket
    {
        [Index(0)]
        public virtual ulong PingId { get; set; }
    }
}