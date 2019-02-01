using System;

namespace ImperialStudio.Core.Api.Networking
{
    public interface INetworkPeer
    {
        bool IsAuthenticated { get; set; }
        ulong SteamId { get; set; }
        TimeSpan Ping { get; set; }
        string Username { get; set; }
        uint Id { get; }
        string Ip { get; }
    }
}