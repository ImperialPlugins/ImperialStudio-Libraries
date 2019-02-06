using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Map;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Scheduling;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Core.Map;

namespace ImperialStudio.Networking.Packets.Handlers
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
            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                m_MapManager.ChangeMap(incomingPacket.MapName);
            }, "Map change to " + incomingPacket.MapName);
        }
    }
}