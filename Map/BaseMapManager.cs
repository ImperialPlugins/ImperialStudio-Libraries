namespace ImperialStudio.Core.Map
{
    public class BaseMapManager : IMapManager
    {
        public string CurrentMap { get; private set; }
        public virtual void ChangeMap(string mapName)
        {
            CurrentMap = mapName;
        }
    }
}