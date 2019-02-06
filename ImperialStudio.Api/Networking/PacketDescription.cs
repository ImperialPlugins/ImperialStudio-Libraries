using ENet;
using ImperialStudio.Api.Networking.Packets;

namespace ImperialStudio.Api.Networking
{
    public class PacketDescription
    {
        public string Name { get; }
        public PacketDirection Direction { get; }
        public byte ChannelId { get; }
        public PacketFlags PacketFlags { get; }
        public bool NeedsAuthentication { get; }

        public PacketDescription(string name, PacketDirection direction, byte channel, PacketFlags packetFlags, bool needsAuthentication = true)
        {
            Name = name;
            Direction = direction;
            ChannelId = channel;
            PacketFlags = packetFlags;
            NeedsAuthentication = needsAuthentication;
        }
    }
}