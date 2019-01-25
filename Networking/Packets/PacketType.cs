namespace ImperialStudio.Core.Networking.Packets
{
    public enum PacketType : byte
    {
        [PacketDescription(PacketDirection.Any, NetworkChannel.PingPong)]
        Ping,

        [PacketDescription(PacketDirection.Any, NetworkChannel.PingPong)]
        Pong,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Main, false)]
        Authenticate,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main)]
        Authenticated,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main)]
        MapChange,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.World)]
        WorldUpdate,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Input)]
        InputUpdate,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main)]
        Terminate
    }
}