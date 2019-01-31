using System.Collections.Generic;
using ImperialStudio.Core.Entities;
using ZeroFormatter;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.WorldUpdate)]
    [ZeroFormattable]
    public class WorldUpdatePacket: IPacket
    {
        [Index(0)]
        public virtual IEnumerable<WorldSpawn> Spawns { get; set; }

        [Index(1)]
        public virtual IDictionary<ushort, byte[]> EntityStates { get; set; }

        [Index(2)]
        public virtual ushort[] Despawns { get; set; }
    }

    [ZeroFormattable]
    public class WorldSpawn
    {
        public WorldSpawn(IEntity entity)
        {
            Id = entity.Id;
            Type = entity.GetType().FullName;
        }

        public WorldSpawn()
        {
            
        }

        [Index(0)]
        public virtual ushort Id { get; set; }

        [Index(1)]
        public virtual string Type { get; set; }
    }
}