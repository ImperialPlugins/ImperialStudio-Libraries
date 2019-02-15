using System.Collections.Generic;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking
{
    public class OutgoingPacket
    {
        public IEnumerable<INetworkPeer> Peers { get; set; }
        public byte PacketId { get; set; }
        public byte[] Data { get; set; }
    }
}