using System.Numerics;

namespace ImperialStudio.Api.Entities
{
    public interface IWorldEntity
    {
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
    }
}