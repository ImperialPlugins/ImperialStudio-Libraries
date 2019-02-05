using System;
using System.Collections.Generic;
using ImperialStudio.Core.Api.Networking.Packets;

namespace ImperialStudio.Core.Api.Networking
{
    public interface IConnectionHandler : IDisposable
    {
        void Send<T>(INetworkPeer peer, T packet, byte packetId) where T : class, IPacket;
        void Send<T>(INetworkPeer peer, T packet) where T : class, IPacket;
        void Send(OutgoingPacket outgoingPacket);
        void Flush();

        IEnumerable<INetworkPeer> GetPeers(bool authenticatedOnly = true);
        IEnumerable<INetworkPeer> GetPendingPeers();

        void RegisterPeer(INetworkPeer networkPeer);
        void UnregisterPeer(INetworkPeer networkPeer);

        void RegisterPacket(byte id, PacketDescription packetDescription);
        PacketDescription GetPacketDescription(byte id);

        INetworkPeer GetPeerByNetworkId(uint peerID, bool authenticatedOnly = true);
        INetworkPeer GetPeerBySteamId(ulong steamId, bool authenticatedOnly = true);
    }
}