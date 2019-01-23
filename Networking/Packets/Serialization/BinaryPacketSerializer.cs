using System.IO;
using BinarySerialization;
using ImperialStudio.Core.Networking.Packets.Handlers;

namespace ImperialStudio.Core.Networking.Packets.Serialization
{
    public class BinaryPacketSerializer : IPacketSerializer
    {
        public byte[] Serialize(IPacket packet) 
        {
            var stream = new MemoryStream();
            var serializer = new BinarySerializer();
            serializer.Serialize(stream, packet);
            try
            {
                return stream.ToArray();
            }
            finally
            {
                stream.Dispose();
            }
        }

        public T Deserialize<T>(byte[] data) where T : IPacket
        {
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;

            var serializer = new BinarySerializer();
            try
            {
                return serializer.Deserialize<T>(stream);
            }
            finally
            {
                stream.Dispose();
            }
        }
    }
}