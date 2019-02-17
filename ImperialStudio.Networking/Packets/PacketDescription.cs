using System;
using ENet;

namespace ImperialStudio.Networking.Packets
{
    public class PacketDescription : ICloneable
    {
        public string Name { get; set; }
        public PacketDirection Direction { get; set; }
        public byte ChannelId { get; set; }
        public PacketFlags PacketFlags { get; set; }
        public bool NeedsAuthentication { get; set; }

        public PacketDescription(string name, PacketDirection direction, byte channel, PacketFlags packetFlags, bool needsAuthentication = true)
        {
            Name = name;
            Direction = direction;
            ChannelId = channel;
            PacketFlags = packetFlags;
            NeedsAuthentication = needsAuthentication;
        }

        public PacketDescription Clone()
        {
            return (PacketDescription) MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}