using System;
using ENet;
using ImperialStudio.Core.Networking.Packets;
using ImperialStudio.Core.Networking.Packets.Handlers;

namespace ImperialStudio.Core.Networking
{
    public interface IConnectionHandler : IDisposable
    {
        void Send(Peer peer, IPacket packet);
        void Send(OutgoingPacket outgoingPacket);
        void Flush();

        bool IsAuthenticated(Peer peer);
        void Authenticate(Peer peer, ulong steamId);
        void Unauthenticate(Peer peer);
    }
}