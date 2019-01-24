using ENet;
using System.Collections.Generic;

namespace ImperialStudio.Core.Networking.Packets
{
    public sealed class OutgoingPacket
    {
        public IEnumerable<NetworkPeer> Peers { get; set; }
        public PacketType PacketType { get; set; }
        public byte[] Data { get; set; }
    }
}