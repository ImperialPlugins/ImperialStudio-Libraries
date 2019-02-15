using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.InputUpdate)]
    [MessagePackObject]
    public class InputUpdatePacket : IPacket
    {
        [Key(0)]
        public virtual int EntityId { get; set; }
        [Key(1)]
        public virtual SVector3? Position { get; set; }
        [Key(2)]
        public virtual SVector3? Rotation { get; set; }
    }
}