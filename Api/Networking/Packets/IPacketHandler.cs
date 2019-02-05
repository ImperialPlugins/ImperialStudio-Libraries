namespace ImperialStudio.Core.Api.Networking.Packets
{
    public interface IPacketHandler
    {
        byte PacketId { get; }

        void HandlePacket(IncomingPacket incomingPacket);
    }
}