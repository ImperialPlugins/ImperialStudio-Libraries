using System;
using System.Collections.Generic;
using System.Linq;
using ImperialStudio.Api.Networking.Packets;
using ImperialStudio.Networking.Packets.Handlers;

namespace ImperialStudio.Networking.Packets
{
    public static class PacketExtensions
    {
        private static readonly Dictionary<Type, PacketTypeAttribute> m_PacketTypeAttributeCache = new Dictionary<Type, PacketTypeAttribute>();
        public static byte GetPacketId(this IPacket packet)
        {
            var type = packet.GetType();

            if (!m_PacketTypeAttributeCache.ContainsKey(type))
            {
                var attribute = (PacketTypeAttribute)type.GetCustomAttributes(typeof(PacketTypeAttribute), false).First();
                m_PacketTypeAttributeCache.Add(type, attribute);
            }

            return m_PacketTypeAttributeCache[type].PacketId;
        }
    }
}