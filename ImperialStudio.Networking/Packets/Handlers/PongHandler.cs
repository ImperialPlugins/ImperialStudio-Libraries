using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Pong)]
    public class PongHandler : BasePacketHandler<PongPacket>
    {
        private readonly IEventBus m_EventBus;

        public PongHandler(IEventBus eventBus, IObjectSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EventBus = eventBus;
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, PongPacket incomingPacket)
        {
            m_EventBus.Emit(this, new PongEvent(sender, incomingPacket));
        }
    }
}