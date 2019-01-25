using ENet;

namespace ImperialStudio.Core.Networking.Packets
{
    public enum PacketType : byte
    {
        [PacketDescription(PacketDirection.Any, NetworkChannel.PingPong, PacketFlags.Reliable)]
        Ping,

        [PacketDescription(PacketDirection.Any, NetworkChannel.PingPong, PacketFlags.Reliable)]
        Pong,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Main, PacketFlags.Reliable, false)]
        Authenticate,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main, PacketFlags.Reliable)]
        Authenticated,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main, PacketFlags.Reliable)]
        MapChange,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.World, PacketFlags.Unsequenced)]
        WorldUpdate,

        [PacketDescription(PacketDirection.ClientToServer, NetworkChannel.Input, PacketFlags.Unsequenced)]
        InputUpdate,

        [PacketDescription(PacketDirection.ServerToClient, NetworkChannel.Main, PacketFlags.Reliable)]
        Terminate
    }
}