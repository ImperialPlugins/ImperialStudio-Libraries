using Castle.Windsor;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Core.UnityEngine.DependencyInjection;
using ImperialStudio.Networking;
using ImperialStudio.Networking.Components;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.Entities
{
    public abstract class ClientSyncUnityGameObjectEntity : UnityGameObjectEntity
    {
        private readonly IWindsorContainer m_Container;
        private EntityTransformSyncComponent m_SyncComponent;

        protected ClientSyncUnityGameObjectEntity(
            IWindsorContainer container,
            IObjectSerializer objectSerializer,
            IConnectionHandler connectionHandler,
            INetworkComponentManager networkComponentManager) : base(objectSerializer, connectionHandler, networkComponentManager)
        {
            m_Container = container;
        }

        protected override void OnOwnerStatusChange(bool isOwner)
        {
            base.OnOwnerStatusChange(isOwner);

            if (isOwner && GameObject != null)
            {
                m_SyncComponent = GameObject.AddComponentWithInjection<EntityTransformSyncComponent>(m_Container);
                GameObject.SetActive(false);
                m_SyncComponent.Entity = this;
                GameObject.SetActive(true);
            }
            else
            {
                if (m_SyncComponent != null)
                {
                    Object.Destroy(m_SyncComponent);
                    m_SyncComponent = null;
                }
            }
        }
    }
}