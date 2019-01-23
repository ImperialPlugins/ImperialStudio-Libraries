using ImperialStudio.Core.Networking.Packets.Handlers;

namespace ImperialStudio.Core.Networking.Packets.Serialization
{
    public interface IPacketSerializer
    {
        byte[] Serialize(IPacket packet);
        T Deserialize<T>(byte[] data) where T : IPacket;
    }
}