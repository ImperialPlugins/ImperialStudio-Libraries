using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Extensions.Logging;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticated)]
    public class AuthenticatedHandler : BasePacketHandler
    {
        private readonly ILogger m_Logger;

        public AuthenticatedHandler(
            IObjectSerializer objectSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger) : base(objectSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_Logger = logger;
        }

        protected override void OnHandleVerifiedPacket(IncomingPacket incomingPacket)
        {
            incomingPacket.Peer.IsAuthenticated = true;
            m_Logger.LogInformation("Successfully authenticated to server.");
        }
    }
}