using ENet;
using ImperialStudio.Core.Api.Entities;
using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Game;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Scheduling;
using ImperialStudio.Core.Api.Serialization;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Serialization;
using NetStack.Serialization;
using System;
using System.Linq;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.WorldUpdate)]
    public class WorldUpdateHandler : BasePacketHandler<WorldUpdatePacket>
    {
        private readonly IEntityManager m_EntityManager;
        private readonly ITaskScheduler m_TaskScheduler;
        private readonly ILogger m_Logger;

        public WorldUpdateHandler(
            IEntityManager entityManager,
            IObjectSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            IEventBus eventBus,
            ITaskScheduler taskScheduler,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EntityManager = entityManager;
            m_TaskScheduler = taskScheduler;
            m_Logger = logger;

            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
                eventBus.Subscribe<NetworkEvent>(this, OnNetworkEvent);
            }
        }

        private void OnNetworkEvent(object sender, NetworkEvent @event)
        {
            if (@event.EnetEvent.Type == EventType.Disconnect || @event.EnetEvent.Type == EventType.Timeout)
            {
                m_EntityManager.DespawnEntities(@event.NetworkPeer);
            }
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, WorldUpdatePacket incomingPacket)
        {
            foreach (var spawn in incomingPacket.Spawns)
            {
                var spawnType = Type.GetType(spawn.Type);
                if (spawnType == null)
                {
                    m_Logger.LogWarning("Could not resolve entity type: " + spawn.Type);
                    continue;
                }

                m_TaskScheduler.ScheduleUpdate(this, () => m_EntityManager.Spawn(spawn.Id, spawnType, spawn.IsOwner), "EntitySpawn[" + spawnType.Name + "]@" + spawn.Id, ExecutionTargetContext.NextFrame);
            }

            m_TaskScheduler.ScheduleUpdate(this, () =>
            {
                foreach (var statePair in incomingPacket.EntityStates.Where(d =>
                    incomingPacket.Spawns.All(e => e.Id != d.Key)))
                {

                    var entity = m_EntityManager.GetEntitiy(statePair.Key);
                    if (entity == null)
                        return;

                    var state = statePair.Value;
                    BitBuffer bitBuffer = new BitBuffer(state.Length);
                    bitBuffer.AddBytes(state);

                    entity.Read(bitBuffer);
                }
            }, "UpdateEntities", ExecutionTargetContext.NextFrame);

            foreach (var despawnId in incomingPacket.Despawns)
            {
                m_EntityManager.Despawn(despawnId);
            }
        }
    }
}