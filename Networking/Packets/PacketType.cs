namespace ImperialStudio.Core.Networking.Packets
{
    public enum PacketType : byte
    {
        [PacketDescription(PacketDirection.Any, NetworkChannel.Main)]
        Ping,

        [PacketDescription(PacketDirection.Any, NetworkChannel.Main)]
        Pong,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Main, false)]
        Authenticate,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main)]
        Authenticated,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Main)]
        AuthenticationFail,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main)]
        MapChange,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.World)]
        WorldUpdate,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Input)]
        InputUpdate,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Chat)]
        ChatInput,  

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Chat)]
        ChatOutput,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main)]
        Terminate
    }
}