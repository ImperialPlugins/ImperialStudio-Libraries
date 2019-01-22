namespace ImperialStudio.Core.Game
{
    public class GamePlatformAccessor : IGamePlatformAccessor
    {
        public GamePlatform GamePlatform { get; }

        public GamePlatformAccessor(GamePlatform gamePlatform)
        {
            GamePlatform = gamePlatform;
        }
    }
}