namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    public interface IPacketHandler
    {
        PacketType PacketType { get; }

        void HandlePacket(IncomingPacket incomingPacket);
    }
}