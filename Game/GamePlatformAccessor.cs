using ImperialStudio.Core.Api.Game;

namespace ImperialStudio.Core.Game
{
    public class GamePlatformAccessor : IGamePlatformAccessor
    {
        public GamePlatformAccessor(GamePlatform gamePlatform)
        {
            GamePlatform = gamePlatform;
        }

        public GamePlatform GamePlatform { get; }
    }
}