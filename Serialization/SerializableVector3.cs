using System;
using UnityEngine;
using ZeroFormatter;

namespace ImperialStudio.Core.Serialization
{
    [ZeroFormattable]
    [Serializable]
    public class SerializableVector3
    {
        public SerializableVector3()
        {
            
        }

        public SerializableVector3(Vector3 unityVector)
        {
            X = unityVector.x;
            Y = unityVector.y;
            Z = unityVector.z;
        }

        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3(vector);
        }

        public static implicit operator Vector3(SerializableVector3 vector)
        {
            return vector.ToUnityVector3();
        }

        [Index(0)]
        public virtual float X { get; set; }
        [Index(1)]
        public virtual float Y { get; set; }
        [Index(2)]
        public virtual float Z { get; set; }

        public Vector3 ToUnityVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
}