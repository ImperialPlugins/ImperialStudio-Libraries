using System;
using System.Collections.Generic;
using NetStack.Unsafe;

namespace ImperialStudio.Core.Networking.Packets
{
    public class OutgoingPacket
    {
        public IEnumerable<NetworkPeer> Peers { get; set; }
        public PacketType PacketType { get; set; }
        public byte[] Data { get; set; }
    }
}