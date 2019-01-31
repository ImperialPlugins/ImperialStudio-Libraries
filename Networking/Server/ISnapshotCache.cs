namespace ImperialStudio.Core.Networking.Server
{
    public interface ISnapshotCache
    {
        byte[] GetSnapshot(NetworkPeer peer, ushort entityId);
        void AddSnapshot(NetworkPeer peer, ushort entityId, byte[] snapShot);
        bool HasSnapshot(NetworkPeer peer, ushort entityId);
        void Clear(NetworkPeer peer);
    }
}