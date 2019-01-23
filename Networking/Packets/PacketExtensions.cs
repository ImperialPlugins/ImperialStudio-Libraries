using System;
using System.Collections.Generic;
using System.Linq;
using ImperialStudio.Core.Networking.Packets.Handlers;

namespace ImperialStudio.Core.Networking.Packets
{
    public static class PacketExtensions
    {
        private static readonly Dictionary<PacketType, PacketDescriptionAttribute> m_PacketDescriptionAttributeCache = new Dictionary<PacketType, PacketDescriptionAttribute>();
        public static PacketDescriptionAttribute GetPacketDescription(this PacketType packetType)
        {
            if (!m_PacketDescriptionAttributeCache.ContainsKey(packetType))
            {
                var type = typeof(PacketType);
                var memInfo = type.GetMember(packetType.ToString());
                var attribute = (PacketDescriptionAttribute)memInfo[0].GetCustomAttributes(typeof(PacketDescriptionAttribute), false).First();
                m_PacketDescriptionAttributeCache.Add(packetType, attribute);
            }

            return m_PacketDescriptionAttributeCache[packetType];
        }

        private static readonly Dictionary<Type, PacketTypeAttribute> m_PacketTypeAttributeCache = new Dictionary<Type, PacketTypeAttribute>();
        public static PacketType GetPacketType(this IPacket packet)
        {
            var type = packet.GetType();

            if (!m_PacketTypeAttributeCache.ContainsKey(type))
            {
                var attribute = (PacketTypeAttribute)type.GetCustomAttributes(typeof(PacketTypeAttribute), false).First();
                m_PacketTypeAttributeCache.Add(type, attribute);
            }

            return m_PacketTypeAttributeCache[type].PacketType;
        }
    }
}