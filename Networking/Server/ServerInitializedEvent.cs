using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Networking.Server
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