using ImperialStudio.Api.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace ImperialStudio.Core.Serialization
{
    public class MessagePackSerializer : IObjectSerializer
    {
        private static readonly MethodInfo m_DeserializeMethodBytes = 
            typeof(MessagePack.MessagePackSerializer)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(d => d.Name == "Deserialize" && d.GetParameters().FirstOrDefault()?.ParameterType == typeof(byte[]));

        private static readonly MethodInfo m_DeserializeMethodArraySegment = 
            typeof(MessagePack.MessagePackSerializer)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(d => d.Name == "Deserialize" && d.GetParameters().FirstOrDefault()?.ParameterType == typeof(ArraySegment<byte>));

        public byte[] Serialize<T>(T packet) where T : class
        {
            return MessagePack.MessagePackSerializer.Serialize(packet);
        }

        public byte[] Serialize(object packet)
        {
            return MessagePack.MessagePackSerializer.Serialize(packet);
        }

        public T Deserialize<T>(byte[] data) where T : class, new()
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(data);
        }

        public T Deserialize<T>(ArraySegment<byte> data) where T : class, new()
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(data);
        }

        public object Deserialize(byte[] data, Type type)
        {
            var genericDeserialize = m_DeserializeMethodBytes.MakeGenericMethod(type);
            return genericDeserialize.Invoke(null, new object[] { data });
        }

        public object Deserialize(ArraySegment<byte> data, Type type)
        {
            var genericDeserialize = m_DeserializeMethodArraySegment.MakeGenericMethod(type);
            return genericDeserialize.Invoke(null, new object[] { data });
        }
    }
}