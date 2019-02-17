using System.Collections.Generic;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking.Packets
{
    public class OutgoingPacket
    {
        public IReadOnlyCollection<INetworkPeer> Peers { get; set; }
        public byte PacketId { get; set; }
        public byte[] Data { get; set; }
        public PacketDescription PacketDescription { get; set; }
    }
}