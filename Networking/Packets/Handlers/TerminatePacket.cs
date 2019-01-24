using BinarySerialization;
using Facepunch.Steamworks;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Terminate)]
    public class TerminatePacket : IPacket
    {
        [FieldOrder(0)]
        public string Reason { get; set; }

        [FieldOrder(1)]
        public ServerAuth.Status AuthFailureReason { get; set; }
    }
}