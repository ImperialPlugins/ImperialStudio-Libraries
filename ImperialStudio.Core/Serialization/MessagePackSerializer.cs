using ImperialStudio.Api.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace ImperialStudio.Core.Serialization
{
    public class MessagePackSerializer : IObjectSerializer
    {
        private static readonly MethodInfo m_DeserializeMethod = typeof(MessagePackSerializer).GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public);

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

        public object Deserialize(byte[] data, Type type)
        {
            var genericDeserialize = m_DeserializeMethod.MakeGenericMethod(type);
            return genericDeserialize.Invoke(null, new object[] { data });
        }
    }
}