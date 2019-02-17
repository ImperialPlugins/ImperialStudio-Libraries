using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Rpc)]
    [MessagePackObject]
    public class RpcPacket : IPacket
    {
        [Key(0)]
        public int ComponentId { get; set; }

        [Key(1)]
        public byte MethodIndex { get; set; }

        [Key(2)]
        public byte[] Arguments { get; set; }
    }
}