using ImperialStudio.Api.Networking.Packets;
using ImperialStudio.Core.Serialization;
using ZeroFormatter;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.InputUpdate)]
    [ZeroFormattable]
    public class InputUpdatePacket : IPacket
    {
        [Index(0)]
        public virtual int EntityId { get; set; }
        [Index(1)]
        public virtual SVector3? Position { get; set; }
        [Index(2)]
        public virtual SVector3? Rotation { get; set; }
    }
}