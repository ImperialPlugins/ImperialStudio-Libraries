using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Networking.Server
{
    public class ServerInitializedEvent: Event
    {
        public ServerListenParameters ListenParameters { get; }

        public ServerInitializedEvent(ServerListenParameters listenParameters)
        {
            ListenParameters = listenParameters;
        }
    }
}