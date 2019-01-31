using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticated)]
    public class AuthenticatedHandler : BasePacketHandler
    {
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly ILogger m_Logger;

        public AuthenticatedHandler(
            IObjectSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ConnectionHandler = connectionHandler;
            m_Logger = logger;
        }

        protected override void OnHandleVerifiedPacket(IncomingPacket incomingPacket)
        {
            incomingPacket.Peer.IsAuthenticated = true;
            m_Logger.LogInformation("Successfully authenticated to server.");
        }
    }
}