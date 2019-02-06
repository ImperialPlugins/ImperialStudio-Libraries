using ImperialStudio.Networking.Client;
using ImperialStudio.Networking.Packets.Handlers;

namespace ImperialStudio.Networking.Entities
{
    public class EntityTransformSyncComponent : MonoBehaviour
    {
        [AutoInject]
        private IConnectionHandler m_ConnectionHandler;

        public IEntity Entity { get; set; }

        private Vector3? m_LastPosition;
        private Vector3? m_LastRotation;

        protected void Update()
        {
            var clientConnectionHandler = m_ConnectionHandler as ClientConnectionHandler;

            if (clientConnectionHandler == null)    
            {
                Destroy(this);
                return;
            }

            if (Entity?.Transform == null)
            {
                return;
            }

            InputUpdatePacket packet = new InputUpdatePacket { EntityId = Entity.Id };

            if (Entity.Transform.position != m_LastPosition)
            {
                packet.Position = Entity.Transform.position;
                m_LastPosition = packet.Position;
            }

            var eulerRotation = Entity.Transform.rotation.eulerAngles;
            if (eulerRotation != m_LastRotation)
            {
                packet.Rotation = eulerRotation;
                m_LastRotation = packet.Rotation;
            }

            if (packet.Position == null && packet.Rotation == null)
            {
                return;
            }

            m_ConnectionHandler.Send(clientConnectionHandler.ServerPeer, packet);
        }
    }
}