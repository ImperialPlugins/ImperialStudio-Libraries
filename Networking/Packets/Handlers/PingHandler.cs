using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Game;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.Ping)]
    public class PingHandler : BasePacketHandler<PingPacket>
    {
        private readonly IEventBus m_EventBus;
        private readonly IConnectionHandler m_ConnectionHandler;

        public PingHandler(IEventBus eventBus, IObjectSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) 
            : base(packetSerializer, gamePlatformAccessor,  connectionHandler, logger)
        {
            m_EventBus = eventBus;
            m_ConnectionHandler = connectionHandler;
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, PingPacket incomingPacket)
        {
            var @event = new PingEvent(sender, incomingPacket);
            m_EventBus.Emit(this, @event);

            if (@event.IsCancelled)
                return;

            PongPacket pongPacket = new PongPacket
            {
                PingId = incomingPacket.PingId
            };

            m_ConnectionHandler.Send(sender, pongPacket);
        }
    }
}