using ImperialStudio.Api.Entities;
using ImperialStudio.Core.UnityEngine.DependencyInjection;
using ImperialStudio.Networking;
using ImperialStudio.Networking.Client;
using ImperialStudio.Networking.Packets.Handlers;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace ImperialStudio.Core.UnityEngine.Entities
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

            if (!(Entity is IWorldEntity worldEntity))
            {
                return;
            }

            InputUpdatePacket packet = new InputUpdatePacket { EntityId = Entity.Id };

            if (worldEntity.Position != m_LastPosition)
            {
                packet.Position = worldEntity.Position;
                m_LastPosition = packet.Position;
            }

            var eulerRotation = worldEntity.Rotation;
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