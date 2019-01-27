namespace ImperialStudio.Core.Networking.Packets.Serialization
{
    public class ZeroFormatterSerializer : IPacketSerializer
    {
        public byte[] Serialize<T>(T packet) where T: class, IPacket
        {
            return ZeroFormatter.ZeroFormatterSerializer.Serialize(packet);
        }

        public T Deserialize<T>(byte[] data) where T : class, IPacket, new()
        {
            return ZeroFormatter.ZeroFormatterSerializer.Deserialize<T>(data);
        }
    }
}