using System.Collections.Generic;
using ImperialStudio.Api.Networking;
using ImperialStudio.Extensions.Util;

namespace ImperialStudio.Networking.Server
{
    public class SnapshotCache : ISnapshotCache
    {
        // <Enet Peer ID, <Entity ID, SnapShot>>
        private readonly Dictionary<uint, Dictionary<int, byte[]>> m_Cache = new Dictionary<uint, Dictionary<int, byte[]>>();

        public byte[] GetSnapshot(INetworkPeer peer, int entityId)
        {
            var peerId = peer.Id;
            if (!m_Cache.ContainsKey(peerId))
            {
                return null;
            }

            var entCache = m_Cache[peerId];
            if (!entCache.ContainsKey(entityId))
            {
                return null;
            }

            return entCache[entityId];
        }

        public void AddSnapshot(INetworkPeer peer, int entityId, byte[] snapShot)
        {
            var peerId = peer.Id;
            if (!m_Cache.ContainsKey(peerId))
            {
                m_Cache.Add(peerId, new Dictionary<int, byte[]>());
            }

            var entCache = m_Cache[peerId];
            entCache.AddOrSet(entityId, snapShot);
        }

        public bool HasSnapshot(INetworkPeer peer, int entityId)
        {
            return m_Cache.ContainsKey(peer.Id) && m_Cache[peer.Id].ContainsKey(entityId);
        }

        public void Clear(INetworkPeer peer)
        {
            m_Cache.Remove(peer.Id);
        }
    }
}