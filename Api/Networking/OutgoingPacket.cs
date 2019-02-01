using System.Collections.Generic;

namespace ImperialStudio.Core.Api.Networking
{
    public class OutgoingPacket
    {
        public IEnumerable<INetworkPeer> Peers { get; set; }
        public byte PacketId { get; set; }
        public byte[] Data { get; set; }
    }
}