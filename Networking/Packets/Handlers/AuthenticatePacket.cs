using BinarySerialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticate)]
    public class AuthenticatePacket : IPacket
    {
        [FieldOrder(0)]
        public ulong SteamId { get; set; }

        [FieldOrder(1)]
        public byte[] Ticket { get; set; }
    }
}