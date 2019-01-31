using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Networking.Events
{
    public class PeerAuthenicatedEvent : Event
    {
        public NetworkPeer NetworkPeer { get; }

        public PeerAuthenicatedEvent(NetworkPeer networkPeer)
        {
            NetworkPeer = networkPeer;
        }        
    }
}