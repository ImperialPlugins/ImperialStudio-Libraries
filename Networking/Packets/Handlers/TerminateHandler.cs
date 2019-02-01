using ImperialStudio.Core.Api.Game;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Serialization;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Client;
using ImperialStudio.Core.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
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