using ImperialStudio.Api.Networking;

namespace ImperialStudio.Api.Entities
{
    public interface IEntity : INetworkComponent
    {
        int Id { get; set; }
        string Name { get; set; }
        uint? OwnerId { get; set; }
        bool IsDisposed { get; }
        void Init();
        void Dispose();
        bool IsOwner { get; set; }
    }
}