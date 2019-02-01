using ImperialStudio.Core.Api.Networking.Packets;
using ZeroFormatter;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.MapChange)]
    [ZeroFormattable]
    public class MapChangePacket : IPacket
    {
        [Index(0)]
        public virtual string MapName { get; set; }
    }
}