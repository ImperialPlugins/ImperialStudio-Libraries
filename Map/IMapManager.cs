namespace ImperialStudio.Core.Map
{
    public interface IMapManager
    {
        string CurrentMap { get; }

        void ChangeMap(string mapName);
    }
}