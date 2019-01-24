using ENet;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Packets.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Pong)]
    public class PongHandler : BasePacketHandler<PongPacket>
    {
        private readonly IEventBus m_EventBus;

        public PongHandler(IEventBus eventBus, IPacketSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EventBus = eventBus;
        }

        protected override void OnHandleVerifiedPacket(NetworkPeer sender, PongPacket incomingPacket)
        {
            m_EventBus.Emit(this, new PongEvent(sender, incomingPacket));
        }
    }
}