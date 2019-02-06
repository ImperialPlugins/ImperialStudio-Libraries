using ENet;
using ImperialStudio.Api.Networking.Packets;

namespace ImperialStudio.Api.Networking
{
    public interface IIncomingNetworkEventHandler
    {
        void ProcessNetworkEvent(Event @event, INetworkPeer networkPeer);
        void RegisterPacketHandler<T>() where T : class, IPacketHandler;
        void EnsureLoaded();
    }
}