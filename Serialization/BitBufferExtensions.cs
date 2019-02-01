using NetStack.Serialization;
using System;
using NetStack.Compression;
using UnityEngine;

namespace ImperialStudio.Core.Serialization
{
    public static class BitBufferExtensions
    {
        public static float ReadSingle(this BitBuffer @this)
        {
            var bytes = new byte[4];
            bytes[0] = @this.ReadByte();
            bytes[1] = @this.ReadByte();
            bytes[2] = @this.ReadByte();
            bytes[3] = @this.ReadByte();

            return BitConverter.ToSingle(bytes, 0);
        }

        public static void AddSingle(this BitBuffer @this, float value)
        {
            var bytes = BitConverter.GetBytes(value);
            @this.AddByte(bytes[0]);
            @this.AddByte(bytes[1]);
            @this.AddByte(bytes[2]);
            @this.AddByte(bytes[3]);
        }

        public static Vector3 ReadVector3(this BitBuffer @this)
        {
            return new Vector3
            {
                x = @this.ReadSingle(),
                y = @this.ReadSingle(),
                z = @this.ReadSingle()
            };
        }

        public static void AddVector3(this BitBuffer @this, Vector3 value)
        {
            @this.AddSingle(value.x);
            @this.AddSingle(value.y);
            @this.AddSingle(value.z);
        }

        public static void AddBytes(this BitBuffer @this, byte[] value)
        {
            foreach (byte val in value)
                @this.AddByte(val);
        }
    }
}