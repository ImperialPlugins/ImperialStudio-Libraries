using System.Collections.Generic;

namespace ImperialStudio.Core.Networking.Server
{
    public class SnapshotCache : ISnapshotCache
    {
        // <Enet Peer ID, <Entity ID, SnapShot>>
        private readonly Dictionary<uint, Dictionary<ushort, byte[]>> m_Cache = new Dictionary<uint, Dictionary<ushort, byte[]>>();

        public byte[] GetSnapshot(NetworkPeer peer, ushort entityId)
        {
            var peerId = peer.EnetPeer.ID;
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

        public void AddSnapshot(NetworkPeer peer, ushort entityId, byte[] snapShot)
        {
            var peerId = peer.EnetPeer.ID;
            if (!m_Cache.ContainsKey(peerId))
            {
                m_Cache.Add(peerId, new Dictionary<ushort, byte[]>());
            }

            var entCache = m_Cache[peerId];
            if (!entCache.ContainsKey(entityId))
            {
                entCache[entityId] = snapShot;
            }
            else
            {
                entCache.Add(entityId, snapShot);
            }
        }

        public bool HasSnapshot(NetworkPeer peer, ushort entityId)
        {
            return m_Cache.ContainsKey(peer.EnetPeer.ID) && m_Cache[peer.EnetPeer.ID].ContainsKey(entityId);
        }

        public void Clear(NetworkPeer peer)
        {
            m_Cache.Remove(peer.EnetPeer.ID);
        }
    }
}