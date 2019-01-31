using NetStack.Serialization;
using UnityEngine;

namespace ImperialStudio.Core.Entities
{
    public interface IEntity
    {
        ushort Id { get; set; }
        string Name { get; }
        ulong Owner { get; set; }
        Transform Transform { get; }
        bool IsDisposed { get; }
        void Init();
        void Dispose();
        int StateSize { get; }
        void Read(BitBuffer newState);
        void Write(BitBuffer buffer, BitBuffer previousState = null);
    }
}