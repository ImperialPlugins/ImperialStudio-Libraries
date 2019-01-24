using System;
using ENet;

namespace ImperialStudio.Core.Networking
{
    public class NetworkPeer
    {
        public NetworkPeer(Peer peer)
        {
            EnetPeer = peer;
        }

        public Peer EnetPeer { get; }
        public bool IsAuthenticated { get; set; }
        public ulong SteamId { get; set; }
        public TimeSpan Ping { get; set; }

        public string Name => $"Peer #{EnetPeer.ID} (Steam: {SteamId})";

        public override string ToString() => Name;
    }
}