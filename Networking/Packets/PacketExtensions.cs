using System.Collections.Generic;
using System.Linq;

namespace ImperialStudio.Core.Networking.Packets
{
    public static class PacketExtensions
    {
        private static readonly Dictionary<EPacket, PacketAttribute> m_AttributeCache = new Dictionary<EPacket, PacketAttribute>();
        public static PacketAttribute GetInfo(this EPacket packet)
        {
            if (!m_AttributeCache.ContainsKey(packet))
            {
                var type = typeof(EPacket);
                var memInfo = type.GetMember(packet.ToString());
                var attribute = (PacketAttribute)memInfo[0].GetCustomAttributes(typeof(PacketAttribute), false).First();
                m_AttributeCache.Add(packet, attribute);
            }

            return m_AttributeCache[packet];
        }
    }
}