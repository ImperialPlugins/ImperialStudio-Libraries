using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Game;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Api.Map;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Scheduling;
using ImperialStudio.Core.Api.Serialization;
using ImperialStudio.Core.Map;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.MapChange)]
    public class MapChangeHandler : BasePacketHandler<MapChangePacket>
    {
        private readonly ITaskScheduler m_TaskScheduler;
        private readonly IMapManager m_MapManager;
        private readonly IConnectionHandler m_ConnectionHandler;

        public MapChangeHandler(ITaskScheduler taskScheduler, IEventBus eventBus, IMapManager mapManager, IObjectSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor, IConnectionHandler connectionHandler, ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_TaskScheduler = taskScheduler;
            m_MapManager = mapManager;
            m_ConnectionHandler = connectionHandler;

            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
                eventBus.Subscribe<MapChangeEvent>(this, OnMapChange);
            }
        }

        private void OnMapChange(object sender, MapChangeEvent @event)
        {
            var packet = new MapChangePacket { MapName = @event.MapName };

            foreach (var peer in m_ConnectionHandler.GetPeers())
            {
                m_ConnectionHandler.Send(peer, packet);
            }
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, MapChangePacket incomingPacket)
        {
            m_TaskScheduler.ScheduleUpdate(this, () =>
            {
                m_MapManager.ChangeMap(incomingPacket.MapName);
            }, "Map change to " + incomingPacket.MapName, ExecutionTargetContext.NextFrame);
        }
    }
}