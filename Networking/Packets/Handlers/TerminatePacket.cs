using Facepunch.Steamworks;
using ZeroFormatter;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Terminate)]
    [ZeroFormattable]
    public class TerminatePacket : IPacket
    {
        [Index(0)]
        public virtual string Reason { get; set; }

        [Index(1)]
        public virtual ServerAuth.Status AuthFailureReason { get; set; }
    }
}