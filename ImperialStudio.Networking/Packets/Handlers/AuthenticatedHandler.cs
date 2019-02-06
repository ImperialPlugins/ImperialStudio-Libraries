using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Core.Logging;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.Authenticated)]
    public class AuthenticatedHandler : BasePacketHandler
    {
        private readonly ILogger m_Logger;

        public AuthenticatedHandler(
            IObjectSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
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