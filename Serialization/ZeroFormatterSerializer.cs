namespace ImperialStudio.Core.Serialization
{
    public class ZeroFormatterSerializer : IObjectSerializer
    {
        public byte[] Serialize<T>(T packet) where T: class
        {
            return ZeroFormatter.ZeroFormatterSerializer.Serialize(packet);
        }

        public T Deserialize<T>(byte[] data) where T : class, new()
        {
            return ZeroFormatter.ZeroFormatterSerializer.Deserialize<T>(data);
        }
    }
}