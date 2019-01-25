using ImperialStudio.Core.CodeAnalysis;
using ImperialStudio.Core.Eventing;
using UnityEngine.SceneManagement;

namespace ImperialStudio.Core.Map
{
    public class MapManager : IMapManager
    {
        private readonly IEventBus m_EventBus;

        public MapManager(IEventBus eventBus)
        {
            m_EventBus = eventBus;
        }

        public string CurrentMap { get; private set; }
        
        [NotThreadSafe]
        public virtual void ChangeMap(string mapName)
        {
            SceneManager.LoadScene(mapName);
            CurrentMap = mapName;

            MapChangeEvent mapChange = new MapChangeEvent(mapName);
            m_EventBus.Emit(this, mapChange);
        }
    }
}