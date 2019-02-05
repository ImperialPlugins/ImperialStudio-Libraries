#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
namespace ZeroFormatter
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::ZeroFormatter.Formatters;
    using global::ZeroFormatter.Internal;
    using global::ZeroFormatter.Segments;
    using global::ZeroFormatter.Comparers;

    public static partial class ZeroFormatterInitializer
    {
        static bool registered = false;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            if(registered) return;
            registered = true;
            // Enums
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::Facepunch.Steamworks.ServerAuth.Status>.Register(new ZeroFormatter.DynamicObjectSegments.Facepunch.Steamworks.ServerAuth_StatusFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Comparers.ZeroFormatterEqualityComparer<global::Facepunch.Steamworks.ServerAuth.Status>.Register(new ZeroFormatter.DynamicObjectSegments.Facepunch.Steamworks.ServerAuth_StatusEqualityComparer());
            
            // Objects
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.AuthenticatePacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.AuthenticatePacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.InputUpdatePacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.InputUpdatePacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.MapChangePacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.MapChangePacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.PingPacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.PingPacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.PongPacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.PongPacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.TerminatePacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.TerminatePacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawnFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldUpdatePacket>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers.WorldUpdatePacketFormatter<ZeroFormatter.Formatters.DefaultResolver>());
            ZeroFormatter.Formatters.Formatter<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>.Register(new ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Serialization.SerializableVector3Formatter<ZeroFormatter.Formatters.DefaultResolver>());
            // Structs
            // Unions
            // Generics
            ZeroFormatter.Formatters.Formatter.RegisterDictionary<ZeroFormatter.Formatters.DefaultResolver, int, byte[]>();
            ZeroFormatter.Formatters.Formatter.RegisterEnumerable<ZeroFormatter.Formatters.DefaultResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>();
            ZeroFormatter.Formatters.Formatter.RegisterEnumerable<ZeroFormatter.Formatters.DefaultResolver, int>();
        }
    }
}
#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
namespace ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Networking.Packets.Handlers
{
    using global::System;
    using global::ZeroFormatter.Formatters;
    using global::ZeroFormatter.Internal;
    using global::ZeroFormatter.Segments;

    public class AuthenticatePacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.AuthenticatePacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.AuthenticatePacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (2 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, ulong>(ref bytes, startOffset, offset, 0, value.SteamId);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, byte[]>(ref bytes, startOffset, offset, 1, value.Ticket);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, string>(ref bytes, startOffset, offset, 2, value.Username);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 2);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.AuthenticatePacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new AuthenticatePacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class AuthenticatePacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.AuthenticatePacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 8, 0, 0 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;

        CacheSegment<TTypeResolver, byte[]> _Ticket;
        CacheSegment<TTypeResolver, string> _Username;

        // 0
        public override ulong SteamId
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, ulong>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, ulong>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }

        // 1
        public override byte[] Ticket
        {
            get
            {
                return _Ticket.Value;
            }
            set
            {
                _Ticket.Value = value;
            }
        }

        // 2
        public override string Username
        {
            get
            {
                return _Username.Value;
            }
            set
            {
                _Username.Value = value;
            }
        }


        public AuthenticatePacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 2, __elementSizes);

            _Ticket = new CacheSegment<TTypeResolver, byte[]>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 1, __binaryLastIndex, __tracker));
            _Username = new CacheSegment<TTypeResolver, string>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 2, __binaryLastIndex, __tracker));
        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (2 + 1));

                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, ulong>(ref targetBytes, startOffset, offset, 0, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);
                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, byte[]>(ref targetBytes, startOffset, offset, 1, ref _Ticket);
                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, string>(ref targetBytes, startOffset, offset, 2, ref _Username);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 2);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class InputUpdatePacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.InputUpdatePacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.InputUpdatePacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (2 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, int>(ref bytes, startOffset, offset, 0, value.EntityId);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>(ref bytes, startOffset, offset, 1, value.Position);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>(ref bytes, startOffset, offset, 2, value.Rotation);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 2);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.InputUpdatePacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new InputUpdatePacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class InputUpdatePacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.InputUpdatePacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 4, 0, 0 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;

        global::ImperialStudio.Core.Serialization.SerializableVector3 _Position;
        global::ImperialStudio.Core.Serialization.SerializableVector3 _Rotation;

        // 0
        public override int EntityId
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, int>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, int>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }

        // 1
        public override global::ImperialStudio.Core.Serialization.SerializableVector3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                __tracker.Dirty();
                _Position = value;
            }
        }

        // 2
        public override global::ImperialStudio.Core.Serialization.SerializableVector3 Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                __tracker.Dirty();
                _Rotation = value;
            }
        }


        public InputUpdatePacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 2, __elementSizes);

            _Position = ObjectSegmentHelper.DeserializeSegment<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>(originalBytes, 1, __binaryLastIndex, __tracker);
            _Rotation = ObjectSegmentHelper.DeserializeSegment<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>(originalBytes, 2, __binaryLastIndex, __tracker);
        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (2 + 1));

                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, int>(ref targetBytes, startOffset, offset, 0, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);
                offset += ObjectSegmentHelper.SerializeSegment<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>(ref targetBytes, startOffset, offset, 1, _Position);
                offset += ObjectSegmentHelper.SerializeSegment<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>(ref targetBytes, startOffset, offset, 2, _Rotation);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 2);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class MapChangePacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.MapChangePacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.MapChangePacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (0 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, string>(ref bytes, startOffset, offset, 0, value.MapName);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 0);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.MapChangePacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new MapChangePacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class MapChangePacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.MapChangePacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 0 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;

        CacheSegment<TTypeResolver, string> _MapName;

        // 0
        public override string MapName
        {
            get
            {
                return _MapName.Value;
            }
            set
            {
                _MapName.Value = value;
            }
        }


        public MapChangePacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 0, __elementSizes);

            _MapName = new CacheSegment<TTypeResolver, string>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 0, __binaryLastIndex, __tracker));
        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (0 + 1));

                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, string>(ref targetBytes, startOffset, offset, 0, ref _MapName);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 0);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class PingPacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.PingPacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.PingPacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (0 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, ulong>(ref bytes, startOffset, offset, 0, value.PingId);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 0);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.PingPacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new PingPacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class PingPacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.PingPacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 8 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;


        // 0
        public override ulong PingId
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, ulong>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, ulong>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }


        public PingPacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 0, __elementSizes);

        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (0 + 1));

                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, ulong>(ref targetBytes, startOffset, offset, 0, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 0);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class PongPacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.PongPacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.PongPacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (0 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, ulong>(ref bytes, startOffset, offset, 0, value.PingId);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 0);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.PongPacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new PongPacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class PongPacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.PongPacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 8 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;


        // 0
        public override ulong PingId
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, ulong>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, ulong>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }


        public PongPacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 0, __elementSizes);

        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (0 + 1));

                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, ulong>(ref targetBytes, startOffset, offset, 0, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 0);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class TerminatePacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.TerminatePacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.TerminatePacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (1 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, string>(ref bytes, startOffset, offset, 0, value.Reason);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, global::Facepunch.Steamworks.ServerAuth.Status>(ref bytes, startOffset, offset, 1, value.AuthFailureReason);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 1);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.TerminatePacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new TerminatePacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class TerminatePacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.TerminatePacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 0, 4 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;

        CacheSegment<TTypeResolver, string> _Reason;

        // 0
        public override string Reason
        {
            get
            {
                return _Reason.Value;
            }
            set
            {
                _Reason.Value = value;
            }
        }

        // 1
        public override global::Facepunch.Steamworks.ServerAuth.Status AuthFailureReason
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, global::Facepunch.Steamworks.ServerAuth.Status>(__originalBytes, 1, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, global::Facepunch.Steamworks.ServerAuth.Status>(__originalBytes, 1, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }


        public TerminatePacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 1, __elementSizes);

            _Reason = new CacheSegment<TTypeResolver, string>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 0, __binaryLastIndex, __tracker));
        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (1 + 1));

                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, string>(ref targetBytes, startOffset, offset, 0, ref _Reason);
                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, global::Facepunch.Steamworks.ServerAuth.Status>(ref targetBytes, startOffset, offset, 1, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 1);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class WorldSpawnFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (2 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, int>(ref bytes, startOffset, offset, 0, value.Id);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, string>(ref bytes, startOffset, offset, 1, value.Type);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, bool>(ref bytes, startOffset, offset, 2, value.IsOwner);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 2);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new WorldSpawnObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class WorldSpawnObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 4, 0, 1 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;

        CacheSegment<TTypeResolver, string> _Type;

        // 0
        public override int Id
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, int>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, int>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }

        // 1
        public override string Type
        {
            get
            {
                return _Type.Value;
            }
            set
            {
                _Type.Value = value;
            }
        }

        // 2
        public override bool IsOwner
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, bool>(__originalBytes, 2, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, bool>(__originalBytes, 2, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }


        public WorldSpawnObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 2, __elementSizes);

            _Type = new CacheSegment<TTypeResolver, string>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 1, __binaryLastIndex, __tracker));
        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (2 + 1));

                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, int>(ref targetBytes, startOffset, offset, 0, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);
                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, string>(ref targetBytes, startOffset, offset, 1, ref _Type);
                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, bool>(ref targetBytes, startOffset, offset, 2, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 2);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }

    public class WorldUpdatePacketFormatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldUpdatePacket>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Networking.Packets.Handlers.WorldUpdatePacket value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (2 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, global::System.Collections.Generic.IEnumerable<global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>>(ref bytes, startOffset, offset, 0, value.Spawns);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, global::System.Collections.Generic.IDictionary<int, byte[]>>(ref bytes, startOffset, offset, 1, value.EntityStates);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, global::System.Collections.Generic.IEnumerable<int>>(ref bytes, startOffset, offset, 2, value.Despawns);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 2);
            }
        }

        public override global::ImperialStudio.Core.Networking.Packets.Handlers.WorldUpdatePacket Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new WorldUpdatePacketObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class WorldUpdatePacketObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Networking.Packets.Handlers.WorldUpdatePacket, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 0, 0, 0 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;

        CacheSegment<TTypeResolver, global::System.Collections.Generic.IEnumerable<global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>> _Spawns;
        CacheSegment<TTypeResolver, global::System.Collections.Generic.IDictionary<int, byte[]>> _EntityStates;
        CacheSegment<TTypeResolver, global::System.Collections.Generic.IEnumerable<int>> _Despawns;

        // 0
        public override global::System.Collections.Generic.IEnumerable<global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn> Spawns
        {
            get
            {
                return _Spawns.Value;
            }
            set
            {
                _Spawns.Value = value;
            }
        }

        // 1
        public override global::System.Collections.Generic.IDictionary<int, byte[]> EntityStates
        {
            get
            {
                return _EntityStates.Value;
            }
            set
            {
                _EntityStates.Value = value;
            }
        }

        // 2
        public override global::System.Collections.Generic.IEnumerable<int> Despawns
        {
            get
            {
                return _Despawns.Value;
            }
            set
            {
                _Despawns.Value = value;
            }
        }


        public WorldUpdatePacketObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 2, __elementSizes);

            _Spawns = new CacheSegment<TTypeResolver, global::System.Collections.Generic.IEnumerable<global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 0, __binaryLastIndex, __tracker));
            _EntityStates = new CacheSegment<TTypeResolver, global::System.Collections.Generic.IDictionary<int, byte[]>>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 1, __binaryLastIndex, __tracker));
            _Despawns = new CacheSegment<TTypeResolver, global::System.Collections.Generic.IEnumerable<int>>(__tracker, ObjectSegmentHelper.GetSegment(originalBytes, 2, __binaryLastIndex, __tracker));
        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (2 + 1));

                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, global::System.Collections.Generic.IEnumerable<global::ImperialStudio.Core.Networking.Packets.Handlers.WorldSpawn>>(ref targetBytes, startOffset, offset, 0, ref _Spawns);
                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, global::System.Collections.Generic.IDictionary<int, byte[]>>(ref targetBytes, startOffset, offset, 1, ref _EntityStates);
                offset += ObjectSegmentHelper.SerializeCacheSegment<TTypeResolver, global::System.Collections.Generic.IEnumerable<int>>(ref targetBytes, startOffset, offset, 2, ref _Despawns);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 2);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }


}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
namespace ZeroFormatter.DynamicObjectSegments.ImperialStudio.Core.Serialization
{
    using global::System;
    using global::ZeroFormatter.Formatters;
    using global::ZeroFormatter.Internal;
    using global::ZeroFormatter.Segments;

    public class SerializableVector3Formatter<TTypeResolver> : Formatter<TTypeResolver, global::ImperialStudio.Core.Serialization.SerializableVector3>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return null;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::ImperialStudio.Core.Serialization.SerializableVector3 value)
        {
            var segment = value as IZeroFormatterSegment;
            if (segment != null)
            {
                return segment.Serialize(ref bytes, offset);
            }
            else if (value == null)
            {
                BinaryUtil.WriteInt32(ref bytes, offset, -1);
                return 4;
            }
            else
            {
                var startOffset = offset;

                offset += (8 + 4 * (2 + 1));
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, float>(ref bytes, startOffset, offset, 0, value.X);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, float>(ref bytes, startOffset, offset, 1, value.Y);
                offset += ObjectSegmentHelper.SerializeFromFormatter<TTypeResolver, float>(ref bytes, startOffset, offset, 2, value.Z);

                return ObjectSegmentHelper.WriteSize(ref bytes, startOffset, offset, 2);
            }
        }

        public override global::ImperialStudio.Core.Serialization.SerializableVector3 Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = BinaryUtil.ReadInt32(ref bytes, offset);
            if (byteSize == -1)
            {
                byteSize = 4;
                return null;
            }
            return new SerializableVector3ObjectSegment<TTypeResolver>(tracker, new ArraySegment<byte>(bytes, offset, byteSize));
        }
    }

    public class SerializableVector3ObjectSegment<TTypeResolver> : global::ImperialStudio.Core.Serialization.SerializableVector3, IZeroFormatterSegment
        where TTypeResolver : ITypeResolver, new()
    {
        static readonly int[] __elementSizes = new int[]{ 4, 4, 4 };

        readonly ArraySegment<byte> __originalBytes;
        readonly global::ZeroFormatter.DirtyTracker __tracker;
        readonly int __binaryLastIndex;
        readonly byte[] __extraFixedBytes;


        // 0
        public override float X
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, float>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, float>(__originalBytes, 0, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }

        // 1
        public override float Y
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, float>(__originalBytes, 1, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, float>(__originalBytes, 1, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }

        // 2
        public override float Z
        {
            get
            {
                return ObjectSegmentHelper.GetFixedProperty<TTypeResolver, float>(__originalBytes, 2, __binaryLastIndex, __extraFixedBytes, __tracker);
            }
            set
            {
                ObjectSegmentHelper.SetFixedProperty<TTypeResolver, float>(__originalBytes, 2, __binaryLastIndex, __extraFixedBytes, value, __tracker);
            }
        }


        public SerializableVector3ObjectSegment(global::ZeroFormatter.DirtyTracker dirtyTracker, ArraySegment<byte> originalBytes)
        {
            var __array = originalBytes.Array;

            this.__originalBytes = originalBytes;
            this.__tracker = dirtyTracker = dirtyTracker.CreateChild();
            this.__binaryLastIndex = BinaryUtil.ReadInt32(ref __array, originalBytes.Offset + 4);

            this.__extraFixedBytes = ObjectSegmentHelper.CreateExtraFixedBytes(this.__binaryLastIndex, 2, __elementSizes);

        }

        public bool CanDirectCopy()
        {
            return !__tracker.IsDirty;
        }

        public ArraySegment<byte> GetBufferReference()
        {
            return __originalBytes;
        }

        public int Serialize(ref byte[] targetBytes, int offset)
        {
            if (__extraFixedBytes != null || __tracker.IsDirty)
            {
                var startOffset = offset;
                offset += (8 + 4 * (2 + 1));

                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, float>(ref targetBytes, startOffset, offset, 0, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);
                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, float>(ref targetBytes, startOffset, offset, 1, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);
                offset += ObjectSegmentHelper.SerializeFixedLength<TTypeResolver, float>(ref targetBytes, startOffset, offset, 2, __binaryLastIndex, __originalBytes, __extraFixedBytes, __tracker);

                return ObjectSegmentHelper.WriteSize(ref targetBytes, startOffset, offset, 2);
            }
            else
            {
                return ObjectSegmentHelper.DirectCopyAll(__originalBytes, ref targetBytes, offset);
            }
        }
    }


}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
namespace ZeroFormatter.DynamicObjectSegments.Facepunch.Steamworks
{
    using global::System;
    using global::System.Collections.Generic;
    using global::ZeroFormatter.Formatters;
    using global::ZeroFormatter.Internal;
    using global::ZeroFormatter.Segments;


    public class ServerAuth_StatusFormatter<TTypeResolver> : Formatter<TTypeResolver, global::Facepunch.Steamworks.ServerAuth.Status>
        where TTypeResolver : ITypeResolver, new()
    {
        public override int? GetLength()
        {
            return 4;
        }

        public override int Serialize(ref byte[] bytes, int offset, global::Facepunch.Steamworks.ServerAuth.Status value)
        {
            return BinaryUtil.WriteInt32(ref bytes, offset, (Int32)value);
        }

        public override global::Facepunch.Steamworks.ServerAuth.Status Deserialize(ref byte[] bytes, int offset, global::ZeroFormatter.DirtyTracker tracker, out int byteSize)
        {
            byteSize = 4;
            return (global::Facepunch.Steamworks.ServerAuth.Status)BinaryUtil.ReadInt32(ref bytes, offset);
        }
    }



    public class ServerAuth_StatusEqualityComparer : IEqualityComparer<global::Facepunch.Steamworks.ServerAuth.Status>
    {
        public bool Equals(global::Facepunch.Steamworks.ServerAuth.Status x, global::Facepunch.Steamworks.ServerAuth.Status y)
        {
            return (Int32)x == (Int32)y;
        }

        public int GetHashCode(global::Facepunch.Steamworks.ServerAuth.Status x)
        {
            return (int)x;
        }
    }



}
#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
