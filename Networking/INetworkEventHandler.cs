using ENet;
using ImperialStudio.Core.Networking.Packets.Handlers;

namespace ImperialStudio.Core.Networking
{
    public interface INetworkEventHandler
    {
        void ProcessNetworkEvent(Event @event, NetworkPeer networkPeer);
        void RegisterPacketHandler<T>() where T : class, IPacketHandler;
        void EnsureLoaded();
    }
}