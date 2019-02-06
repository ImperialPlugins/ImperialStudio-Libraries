using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Networking.Client;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.Terminate)]
    public class TerminateHandler : BasePacketHandler<TerminatePacket>
    {
        private readonly IConnectionHandler m_ConnectionHandler;

        public TerminateHandler(IObjectSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ConnectionHandler = connectionHandler;
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, TerminatePacket incomingPacket)
        {
            var clientConnection = (ClientConnectionHandler) m_ConnectionHandler;
            clientConnection.PendingTerminate = true;
            clientConnection.Disconnect();
        }
    }
}