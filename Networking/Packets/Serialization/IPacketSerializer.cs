using System;

namespace ImperialStudio.Core.Networking.Packets.Serialization
{
    public interface IPacketSerializer
    {
        byte[] Serialize<T>(T packet) where T : class, IPacket;
        T Deserialize<T>(byte[] data) where T : class, IPacket, new();
    }
}