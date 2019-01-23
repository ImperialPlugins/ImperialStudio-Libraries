using System;

namespace ImperialStudio.Core.Networking.Packets
{
    public class PacketDescriptionAttribute : Attribute
    {
        public PacketDirection Direction { get; }
        public NetworkChannel Channel { get; }

        public bool NeedsAuthentication { get;  }

        public PacketDescriptionAttribute(PacketDirection direction, NetworkChannel channel, bool needsAuthentication = true)
        {
            Direction = direction;
            Channel = channel;
            NeedsAuthentication = needsAuthentication;
        }
    }
}