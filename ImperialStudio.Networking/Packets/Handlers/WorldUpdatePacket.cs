using System.Collections.Generic;
using ImperialStudio.Api.Entities;
using MessagePack;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.WorldUpdate)]
    [MessagePackObject]
    public class WorldUpdatePacket: IPacket
    {
        [Key(0)]
        public virtual IEnumerable<WorldSpawn> Spawns { get; set; }

        [Key(1)]
        public virtual IDictionary<int, byte[]> EntityStates { get; set; }

        [Key(2)]
        public virtual IEnumerable<int> Despawns { get; set; }
    }

    [MessagePackObject]
    public class WorldSpawn
    {
        public WorldSpawn(IEntity entity)
        {
            Id = entity.Id;
            Type = entity.GetType().AssemblyQualifiedName;
        }

        public WorldSpawn()
        {
            
        }

        [Key(0)]
        public virtual int Id { get; set; }

        [Key(1)]
        public virtual string Type { get; set; }
    }
}