using ENet;

namespace ImperialStudio.Core.Networking.Packets
{
    public interface IPacketHandler
    {
        void HandlePacket(Peer peer, EPacket packet, byte[] packetData);
    }
}