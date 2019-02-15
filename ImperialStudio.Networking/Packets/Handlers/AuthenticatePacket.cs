using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticate)]
    [MessagePackObject]
    public class AuthenticatePacket : IPacket
    {
        [Key(0)]
        public virtual ulong SteamId { get; set; }

        [Key(1)]
        public virtual byte[] Ticket { get; set; }

        [Key(2)]
        public virtual string Username { get; set; }
    }
}