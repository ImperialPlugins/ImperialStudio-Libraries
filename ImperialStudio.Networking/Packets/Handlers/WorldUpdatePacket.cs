using System.Collections.Generic;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.WorldUpdate)]
    [ZeroFormattable]
    public class WorldUpdatePacket: IPacket
    {
        [Index(0)]
        public virtual IEnumerable<WorldSpawn> Spawns { get; set; }

        [Index(1)]
        public virtual IDictionary<int, byte[]> EntityStates { get; set; }

        [Index(2)]
        public virtual IEnumerable<int> Despawns { get; set; }
    }

    [ZeroFormattable]
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

        [Index(0)]
        public virtual int Id { get; set; }

        [Index(1)]
        public virtual string Type { get; set; }

        [Index(2)]
        public virtual bool IsOwner { get; set; }
    }
}