﻿namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.MapChange)]
    [ZeroFormattable]
    public class MapChangePacket : IPacket
    {
        [Index(0)]
        public virtual string MapName { get; set; }
    }
}