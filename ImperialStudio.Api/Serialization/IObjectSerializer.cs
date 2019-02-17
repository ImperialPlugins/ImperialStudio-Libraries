using System;

namespace ImperialStudio.Api.Serialization
{
    public interface IObjectSerializer
    {
        byte[] Serialize<T>(T packet) where T : class;
        byte[] Serialize(object packet);

        T Deserialize<T>(byte[] data) where T : class, new();
        T Deserialize<T>(ArraySegment<byte> data) where T : class, new();

        object Deserialize(byte[] data, Type type);
        object Deserialize(ArraySegment<byte> data, Type type);
    }
}