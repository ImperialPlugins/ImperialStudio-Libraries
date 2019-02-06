using ImperialStudio.Api.Eventing;
using ImperialStudio.Core.Events;
using ImperialStudio.Core.UnityEngine.DependencyInjection;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.Eventing
{
    public class EventListenerComponent : MonoBehaviour
    {
        [AutoInject]
        private IEventBus m_EventBus;

        private void OnApplicationQuit()
        {
            m_EventBus.Emit(this, new ApplicationQuitEvent());
        }
    }
}