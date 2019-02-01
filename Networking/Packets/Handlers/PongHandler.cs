using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Game;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.Pong)]
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