using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Networking;

namespace ImperialStudio.Networking.Events
{
    public class PeerAuthenicatedEvent : IEvent
    {
        public INetworkPeer NetworkPeer { get; }

        public PeerAuthenicatedEvent(INetworkPeer networkPeer)
        {
            NetworkPeer = networkPeer;
        }        
    }
}