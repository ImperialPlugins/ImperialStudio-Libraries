using ENet;
using ImperialStudio.Api.Networking;
using ImperialStudio.Networking.Packets;

namespace ImperialStudio.Networking
{
    public interface IIncomingNetworkEventHandler
    {
        void ProcessNetworkEvent(Event @event, INetworkPeer networkPeer);
        void RegisterPacketHandler<T>() where T : class, IPacketHandler;
        void EnsureLoaded();
    }
}