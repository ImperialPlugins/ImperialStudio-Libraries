using System;
using System.Linq;
using ENet;
using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Scheduling;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Extensions.Logging;
using ImperialStudio.Networking.Events;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.WorldUpdate)]
    public class WorldUpdateHandler : BasePacketHandler<WorldUpdatePacket>
    {
        private readonly IEntityManager m_EntityManager;
        private readonly ITaskScheduler m_TaskScheduler;
        private readonly ILogger m_Logger;

        public WorldUpdateHandler(
            IEntityManager entityManager,
            IObjectSerializer objectSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            IEventBus eventBus,
            ITaskScheduler taskScheduler,
            ILogger logger) : base(objectSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EntityManager = entityManager;
            m_TaskScheduler = taskScheduler;
            m_Logger = logger;

            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
                eventBus.Subscribe<ENetNetworkEvent>(this, OnNetworkEvent);
            }
        }

        private void OnNetworkEvent(object sender, ENetNetworkEvent @event)
        {
            if (@event.Event.Type == EventType.Disconnect || @event.Event.Type == EventType.Timeout)
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

                m_TaskScheduler.RunOnMainThread(this, () => m_EntityManager.Spawn(spawn.Id, spawnType), "EntitySpawn[" + spawnType.Name + "]@" + spawn.Id);
            }

            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                foreach (var statePair in incomingPacket.EntityStates.Where(d =>
                    incomingPacket.Spawns.All(e => e.Id != d.Key)))
                {
                    var entity = m_EntityManager.GetEntitiy(statePair.Key);
                    if (entity == null)
                        return;

                    var state = statePair.Value;

                    Span<byte> stateSpan = new Span<byte>(state);
                    entity.Read(stateSpan);
                }
            }, "UpdateEntities");

            foreach (var despawnId in incomingPacket.Despawns)
            {
                m_EntityManager.Despawn(despawnId);
            }
        }
    }
}