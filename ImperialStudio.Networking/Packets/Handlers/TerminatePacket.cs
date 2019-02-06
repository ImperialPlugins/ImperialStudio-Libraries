using Facepunch.Steamworks;
using ImperialStudio.Api.Networking.Packets;
using ZeroFormatter;

namespace ImperialStudio.Networking.Packets.Handlers
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