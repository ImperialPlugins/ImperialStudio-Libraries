using System;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketTypeAttribute: Attribute
    {
        public byte PacketId { get; }

        public PacketTypeAttribute(PacketType packetType)
        {
            PacketId = (byte) packetType;
        }

        public PacketTypeAttribute(byte packetId)
        {
            PacketId = packetId;
        }
    }
}