namespace ImperialStudio.Networking.Server
{
    public sealed class ServerListenParameters
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string Map { get; set; }
        public string Name { get; set; }

        public byte MaxPlayers { get; set; }
    }
}