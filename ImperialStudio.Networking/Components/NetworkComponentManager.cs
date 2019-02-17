using ImperialStudio.Api.Networking;
using System;
using System.Collections.Generic;

namespace ImperialStudio.Networking.Components
{
    public class NetworkComponentManager : INetworkComponentManager
    {
        private readonly Dictionary<int, WeakReference<INetworkComponent>> m_NetworkComponents = new Dictionary<int, WeakReference<INetworkComponent>>();

        public void RegisterComponent(int id, INetworkComponent instance)
        {
            if (m_NetworkComponents.ContainsKey(id))
            {
                var previousInstance = m_NetworkComponents[id];

                if (!previousInstance.TryGetTarget(out _))
                {
                    m_NetworkComponents.Remove(id);
                }
                else
                {
                    throw new Exception("Component already registered: " + id);
                }
            }

            m_NetworkComponents.Add(id, new WeakReference<INetworkComponent>(instance));
        }

        public void UnregisterComponent(int id)
        {
            if (m_NetworkComponents.ContainsKey(id))
                m_NetworkComponents.Remove(id);
        }

        public INetworkComponent GetComponent(int componentId)
        {
            if (!m_NetworkComponents.ContainsKey(componentId))
            {
                throw new Exception("Component not found: " + componentId);
            }

            var componentReference = m_NetworkComponents[componentId];
            if (componentReference.TryGetTarget(out var component))
                return component;

            throw new Exception("Component does not exist anymore: " + componentId);
        }

        public INetworkComponent this[int componentId] => GetComponent(componentId);
    }
}