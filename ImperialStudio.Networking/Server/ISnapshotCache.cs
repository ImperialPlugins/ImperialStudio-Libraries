namespace ImperialStudio.Networking.Server
{
    public interface ISnapshotCache
    {
        byte[] GetSnapshot(INetworkPeer peer, int entityId);
        void AddSnapshot(INetworkPeer peer, int entityId, byte[] snapShot);
        bool HasSnapshot(INetworkPeer peer, int entityId);
        void Clear(INetworkPeer peer);
    }
}