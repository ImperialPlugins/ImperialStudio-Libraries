using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Map
{
    public class MapChangeEvent : Event
    {
        public string MapName { get; }

        public MapChangeEvent(string mapName)
        {
            MapName = mapName;
        }
    }
}