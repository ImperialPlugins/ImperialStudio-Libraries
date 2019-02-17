using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking.Components
{
    public interface INetworkComponentManager
    {
        void RegisterComponent(int id, INetworkComponent instance);
        void UnregisterComponent(int id);
        INetworkComponent GetComponent(int componentId);

        INetworkComponent this[int componentId] { get; }
    }
}