namespace ImperialStudio.Core.Api.Networking.Packets
{
    public interface IPacketHandler
    {
        byte PacketType { get; }

        void HandlePacket(IncomingPacket incomingPacket);
    }
}