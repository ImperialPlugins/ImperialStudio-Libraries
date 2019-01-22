using ImperialStudio.Core.Game;

namespace ImperialStudio.Core.Networking.Packets
{
    public enum EPacket : byte
    {
        [Packet(GamePlatform.Server | GamePlatform.Client, 0)]
        Ping,

        [Packet(GamePlatform.Server | GamePlatform.Client, 0)]
        Pong,

        [Packet(GamePlatform.Client, 1)]
        Authenticate,

        [Packet(GamePlatform.Server, 0)]
        AuthenticationSuccess,

        [Packet(GamePlatform.Client, 1)]
        AuthenticationFail,

        [Packet(GamePlatform.Server, 0)]
        MapChange,

        [Packet(GamePlatform.Server, 0)]
        WorldUpdate,

        [Packet(GamePlatform.Client, 1)]
        InputUpdate,

        [Packet(GamePlatform.Client, 2)]
        ChatInput,  

        [Packet(GamePlatform.Client, 3)]
        ChatOutput,

        [Packet(GamePlatform.Server, 0)]
        Terminate
    }
}