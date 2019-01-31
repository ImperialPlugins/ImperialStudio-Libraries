namespace ImperialStudio.Core.Serialization
{
    public interface IObjectSerializer
    {
        byte[] Serialize<T>(T packet) where T : class;
        T Deserialize<T>(byte[] data) where T : class, new();
    }
}