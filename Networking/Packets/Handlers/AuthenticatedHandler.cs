using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Packets.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticated)]
    public class AuthenticatedHandler : BasePacketHandler
    {
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly ILogger m_Logger;

        public AuthenticatedHandler(
            IPacketSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ConnectionHandler = connectionHandler;
            m_Logger = logger;
        }

        protected override void OnHandleVerifiedPacket(IncomingPacket incomingPacket)
        {
            m_ConnectionHandler.Authenticate(incomingPacket.Peer, 0);
            m_Logger.LogInformation("Successfully authenticated to server.");
        }
    }
}