using System;
using ENet;
using ImperialStudio.Core.Api.Networking;

namespace ImperialStudio.Core.Networking
{
    public class NetworkPeer : INetworkPeer
    {
        public NetworkPeer(Peer enetPeer)
        {
            EnetPeer = enetPeer;
        }

        public Peer EnetPeer { get; set; }
        public bool IsAuthenticated { get; set; }
        public ulong SteamId { get; set; }
        public TimeSpan Ping { get; set; }
        public string Username { get; set; }
        public uint Id => EnetPeer.ID;
        public string Ip => EnetPeer.IP;
        public ushort Port => EnetPeer.Port;
        public override string ToString() => $"Peer #{Id} (Steam: {SteamId})";
    }
}