﻿namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.InputUpdate)]
    [ZeroFormattable]
    public class InputUpdatePacket : IPacket
    {
        [Index(0)]
        public virtual int EntityId { get; set; }
        [Index(1)]
        public virtual SerializableVector3 Position { get; set; }
        [Index(2)]
        public virtual SerializableVector3 Rotation { get; set; }
    }
}