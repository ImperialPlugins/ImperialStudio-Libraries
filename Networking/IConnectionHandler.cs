using Facepunch.Steamworks;
using ImperialStudio.Core.Networking.Packets;
using ImperialStudio.Core.Networking.Packets.Handlers;
using System;
using System.Collections.Generic;

namespace ImperialStudio.Core.Networking
{
    public interface IConnectionHandler : IDisposable
    {
        void Send<T>(NetworkPeer peer, T packet) where T: class, IPacket;
        void Send(OutgoingPacket outgoingPacket);
        void Flush();

        IEnumerable<NetworkPeer> GetPeers();
        void RegisterPeer(NetworkPeer networkPeer);
        void UnregisterPeer(NetworkPeer networkPeer);

        NetworkPeer GetPeerByNetworkId(uint peerID);
        NetworkPeer GetPeerBySteamId(ulong steamId);
    }
}