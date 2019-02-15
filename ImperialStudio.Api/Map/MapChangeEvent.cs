using ImperialStudio.Api.Eventing;

namespace ImperialStudio.Api.Map
{
    public class MapChangeEvent : IEvent
    {
        public string MapName { get; }

        public MapChangeEvent(string mapName)
        {
            MapName = mapName;
        }
    }
}