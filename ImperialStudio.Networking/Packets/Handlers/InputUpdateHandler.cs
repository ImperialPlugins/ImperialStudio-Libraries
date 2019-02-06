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
            if (entity?.Transform == null)
            {
                return;
            }

            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                if (incomingPacket.Position != null)
                {
                    entity.Transform.position = incomingPacket.Position;
                }

                if (incomingPacket.Rotation != null)
                {
                    entity.Transform.rotation = Quaternion.Euler(incomingPacket.Rotation);
                }
            }, "InputUpdate-" + sender.Id);
        }
    }
}