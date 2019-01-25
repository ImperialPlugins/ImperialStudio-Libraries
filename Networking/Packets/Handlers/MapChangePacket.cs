using BinarySerialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.MapChange)]
    public class MapChangePacket : IPacket
    {
        [FieldOrder(0)]
        public string MapName { get; set; }
    }
}