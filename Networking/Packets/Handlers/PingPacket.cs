using BinarySerialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Ping)]
    public class PingPacket : IPacket
    {
        [FieldOrder(0)]
        public ulong PingId { get; set; }
    }
}