using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Events;
using UnityEngine;

namespace ImperialStudio.Core.Eventing
{
    public class EventListener : MonoBehaviour
    {
        [AutoInject]
        private IEventBus m_EventBus;

        private void OnApplicationQuit()
        {
            m_EventBus.Emit(this, new ApplicationQuitEvent());
        }
    }
}