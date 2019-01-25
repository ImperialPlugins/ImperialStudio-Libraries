using System;
using ENet;

namespace ImperialStudio.Core.Networking.Packets
{
    public class PacketDescriptionAttribute : Attribute
    {
        public PacketDirection Direction { get; }
        public NetworkChannel Channel { get; }
        public PacketFlags PacketFlags { get; }

        public bool NeedsAuthentication { get;  }

        public PacketDescriptionAttribute(PacketDirection direction, NetworkChannel channel, PacketFlags packetFlags, bool needsAuthentication = true)
        {
            Direction = direction;
            Channel = channel;
            PacketFlags = packetFlags;
            NeedsAuthentication = needsAuthentication;
        }
    }
}