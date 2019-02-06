namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Ping)]
    [ZeroFormattable]
    public class PingPacket : IPacket
    {
        [Index(0)]
        public virtual ulong PingId { get; set; }
    }
}