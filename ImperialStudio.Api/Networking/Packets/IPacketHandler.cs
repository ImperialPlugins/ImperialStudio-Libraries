namespace ImperialStudio.Api.Networking.Packets
{
    public interface IPacketHandler
    {
        byte PacketId { get; }

        void HandlePacket(IncomingPacket incomingPacket);
    }
}