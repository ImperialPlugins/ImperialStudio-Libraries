using ZeroFormatter;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticate)]
    [ZeroFormattable]
    public class AuthenticatePacket : IPacket
    {
        [Index(0)]
        public virtual ulong SteamId { get; set; }

        [Index(1)]
        public virtual byte[] Ticket { get; set; }

        [Index(2)]
        public virtual string Username { get; set; }
    }
}