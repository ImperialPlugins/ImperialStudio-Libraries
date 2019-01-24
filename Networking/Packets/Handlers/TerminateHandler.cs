using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Client;
using ImperialStudio.Core.Networking.Packets.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Terminate)]
    public class TerminateHandler : BasePacketHandler<TerminatePacket>
    {
        private readonly IConnectionHandler m_ConnectionHandler;

        public TerminateHandler(IPacketSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ConnectionHandler = connectionHandler;
        }

        protected override void OnHandleVerifiedPacket(NetworkPeer sender, TerminatePacket incomingPacket)
        {
            var clientConnection = (ClientConnectionHandler) m_ConnectionHandler;
            clientConnection.Disconnect();
        }
    }
}