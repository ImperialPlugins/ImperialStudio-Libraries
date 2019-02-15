using System;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Map;
using ImperialStudio.Api.Scheduling;
using UnityEngine.SceneManagement;

namespace ImperialStudio.Core.UnityEngine.Map
{
    public class MapManager : IMapManager
    {
        private readonly IGamePlatformAccessor m_GamePlatformAccessor;
        private readonly IEventBus m_EventBus;
        private readonly ITaskScheduler m_TaskScheduler;

        public MapManager(
            IGamePlatformAccessor gamePlatformAccessor,
            IEventBus eventBus,
            ITaskScheduler taskScheduler)
        {
            m_GamePlatformAccessor = gamePlatformAccessor;
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
            if (m_GamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
                throw new Exception("Can not go to Main Menu on server!");
            }

            if (CurrentMap != "MainMenu")
                ChangeMap("MainMenu");
        }
    }
}