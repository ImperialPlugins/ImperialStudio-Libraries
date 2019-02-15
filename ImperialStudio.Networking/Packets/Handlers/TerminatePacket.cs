using Facepunch.Steamworks;
using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Terminate)]
    [MessagePackObject]
    public class TerminatePacket : IPacket
    {
        [Key(0)]
        public virtual string Reason { get; set; }

        [Key(1)]
        public virtual ServerAuth.Status AuthFailureReason { get; set; }
    }
}