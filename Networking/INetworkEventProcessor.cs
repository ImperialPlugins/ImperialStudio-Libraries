using ENet;
using ImperialStudio.Core.Networking.Packets.Handlers;

namespace ImperialStudio.Core.Networking
{
    public interface INetworkEventProcessor
    {
        void ProcessEvent(Event @event);
        void RegisterPacketHandler<T>() where T : class, IPacketHandler;
    }
}