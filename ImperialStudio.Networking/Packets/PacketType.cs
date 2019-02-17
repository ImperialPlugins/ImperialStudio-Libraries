namespace ImperialStudio.Networking.Packets
{
    public enum PacketType : byte
    {
        Ping,
        Pong,
        Authenticate,
        Authenticated,
        MapChange,
        WorldUpdate,
        InputUpdate,
        Terminate,
        Rpc
    }
}