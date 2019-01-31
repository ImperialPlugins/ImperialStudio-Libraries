using System;
using ImperialStudio.Core.Entities;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Serialization;
using NetStack.Serialization;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.WorldUpdate)]
    public class WorldUpdateHandler : BasePacketHandler<WorldUpdatePacket>
    {
        private readonly IEntityManager m_EntityManager;

        public WorldUpdateHandler(
            IEntityManager entityManager,
            IObjectSerializer packetSerializer, 
            IGamePlatformAccessor gamePlatformAccessor, 
            IConnectionHandler connectionHandler, 
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EntityManager = entityManager;
        }

        protected override void OnHandleVerifiedPacket(NetworkPeer sender, WorldUpdatePacket incomingPacket)
        {
            foreach (var spawn in incomingPacket.Spawns)
            {
                var type = Type.GetType(spawn.Type);
                m_EntityManager.Spawn(spawn.Id, type);
            }

            foreach (var statePair in incomingPacket.EntityStates)
            {
                var entity = m_EntityManager.GetEntitiy(statePair.Key);
                if(entity == null)
                    continue;

                var state = statePair.Value;
                BitBuffer bitBuffer = new BitBuffer(state.Length);
                bitBuffer.AddBytes(state);

                entity.Read(bitBuffer);
            }

            foreach (var despawnId in incomingPacket.Despawns)
            {
                m_EntityManager.Despawn(despawnId);
            }
        }
    }
}