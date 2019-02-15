using System;

namespace ImperialStudio.Networking.Packets
{
    [Flags]
    public enum PacketDirection
    {
        ServerToClient = 1,
        ClientToServer = 2,
        ClientToClient = 4,

        Any = ServerToClient | ClientToServer | ClientToClient
    }
}