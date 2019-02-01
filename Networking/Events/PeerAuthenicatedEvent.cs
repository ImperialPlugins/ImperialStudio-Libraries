using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Networking.Events
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