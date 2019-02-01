using ENet;
using ImperialStudio.Core.Api.Networking.Packets;

namespace ImperialStudio.Core.Api.Networking
{
    public interface IIncomingNetworkEventHandler
    {
        void ProcessNetworkEvent(Event @event, INetworkPeer networkPeer);
        void RegisterPacketHandler<T>() where T : class, IPacketHandler;
        void EnsureLoaded();
    }
}