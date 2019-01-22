using System;
using ImperialStudio.Core.Networking.Packets;

namespace ImperialStudio.Core.Networking
{
    public interface IConnectionHandler: IDisposable
    {
        void EnqueueOutgoing(OutgoingPacket outgoingPacket);
        void Flush();
    }
}