using System;
using ImperialStudio.Networking.Packets;

namespace ImperialStudio.Networking.Rpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RpcAttribute : Attribute
    {
        public PacketDirection Direction { get; }

        public RpcAttribute(PacketDirection direction)
        {
            Direction = direction;
        }
    }
}