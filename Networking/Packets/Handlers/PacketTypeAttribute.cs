using System;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketTypeAttribute: Attribute
    {
        public PacketType PacketType { get; }

        public PacketTypeAttribute(PacketType packetType)
        {
            PacketType = packetType;
        }
    }
}