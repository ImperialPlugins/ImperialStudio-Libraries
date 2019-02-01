using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Map;
using ImperialStudio.Core.Api.Scheduling;
using UnityEngine.SceneManagement;

namespace ImperialStudio.Core.Map
{
    public class MapManager : IMapManager
    {
        private readonly IEventBus m_EventBus;
        private readonly ITaskScheduler m_TaskScheduler;

        public MapManager(
            IEventBus eventBus,
            ITaskScheduler taskScheduler)
        {
            m_EventBus = eventBus;
            m_TaskScheduler = taskScheduler;
        }

        public string CurrentMap { get; private set; }

        public virtual void ChangeMap(string mapName)
        {
            m_TaskScheduler.RunOnMainThread(this, () =>
            {
                SceneManager.LoadScene(mapName);
                CurrentMap = mapName;

                MapChangeEvent mapChange = new MapChangeEvent(mapName);
                m_EventBus.Emit(this, mapChange);
            }, "ChangeMap-" + mapName);
        }

        public void GoToMainMenu()
        {
            ChangeMap("MainMenu");
        }
    }
}