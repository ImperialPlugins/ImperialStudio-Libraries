using ImperialStudio.Api.Eventing;

namespace ImperialStudio.Networking.Server
{
    public class ServerInitializedEvent: IEvent
    {
        public ServerListenParameters ListenParameters { get; }

        public ServerInitializedEvent(ServerListenParameters listenParameters)
        {
            ListenParameters = listenParameters;
        }
    }
}