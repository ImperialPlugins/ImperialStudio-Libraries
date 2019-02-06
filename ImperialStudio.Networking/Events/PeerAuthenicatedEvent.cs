using ImperialStudio.Api.Networking;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Networking.Events
{
    public class PeerAuthenicatedEvent : Event
    {
        public INetworkPeer NetworkPeer { get; }

        public PeerAuthenicatedEvent(INetworkPeer networkPeer)
        {
            NetworkPeer = networkPeer;
        }        
    }
}