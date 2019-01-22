using System.Collections.Generic;
using ENet;

namespace ImperialStudio.Core.Networking.Packets
{
    public struct OutgoingPacket
    {
        public IEnumerable<Peer> Peers { get; set; }
        public Packet Packet { get; set; }
        public byte ChannelId { get; set; }
    }
}