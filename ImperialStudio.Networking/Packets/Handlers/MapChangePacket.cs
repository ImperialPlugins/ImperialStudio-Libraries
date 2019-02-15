using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.MapChange)]
    [MessagePackObject]
    public class MapChangePacket : IPacket
    {
        [Key(0)]
        public virtual string MapName { get; set; }
    }
}