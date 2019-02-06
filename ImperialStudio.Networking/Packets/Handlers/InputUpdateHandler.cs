using System.Numerics;
using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Scheduling;
using ImperialStudio.Api.Serialization;
using ILogger = ImperialStudio.Api.Logging.ILogger;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.InputUpdate)]
    public class InputUpdateHandler : BasePacketHandler<InputUpdatePacket>
    {
        private readonly IEntityManager m_EntityManager;
        private readonly ITaskScheduler m_TaskScheduler;

        public InputUpdateHandler(
            IObjectSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            IEntityManager entityManager,
            ITaskScheduler taskScheduler,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EntityManager = entityManager;
            m_TaskScheduler = taskScheduler;
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, InputUpdatePacket incomingPacket)
        {
            var entity = m_EntityManager.GetEntitiy(incomingPacket.EntityId);
            if (!(entity is IWorldEntity worldEntity))
            {
                return;
            }

            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                if (incomingPacket.Position != null)
                {
                    worldEntity.Position = incomingPacket.Position.Value;
                }

                if (incomingPacket.Rotation != null)
                {
                    worldEntity.Rotation = incomingPacket.Rotation.Value;
                }
            }, "InputUpdate-" + sender.Id);
        }
    }
}