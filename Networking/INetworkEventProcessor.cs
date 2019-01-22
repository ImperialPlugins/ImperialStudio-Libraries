using ENet;

namespace ImperialStudio.Core.Networking
{
    public interface INetworkEventProcessor
    {
        void ProcessEvent(Event @event);
    }
}