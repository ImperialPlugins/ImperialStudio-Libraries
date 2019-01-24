using ENet;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Packets.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Ping)]
    public class PingHandler : BasePacketHandler<PingPacket>
    {
        private readonly IEventBus m_EventBus;
        private readonly IConnectionHandler m_ConnectionHandler;

        public PingHandler(IEventBus eventBus, IPacketSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) 
            : base(packetSerializer, gamePlatformAccessor,  connectionHandler, logger)
        {
            m_EventBus = eventBus;
            m_ConnectionHandler = connectionHandler;
        }

        protected override void OnHandleVerifiedPacket(NetworkPeer sender, PingPacket incomingPacket)
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