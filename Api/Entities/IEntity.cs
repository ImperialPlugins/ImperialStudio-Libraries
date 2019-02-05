﻿using ImperialStudio.Core.Api.Networking;
using NetStack.Serialization;
using UnityEngine;

namespace ImperialStudio.Core.Api.Entities
{
    public interface IEntity
    {
        int Id { get; set; }
        string Name { get; set; }
        INetworkPeer Owner { get; set; }
        Transform Transform { get; }
        bool IsDisposed { get; }
        void Init();
        void Dispose();
        bool IsOwner { get; set; }
        void Read(BitBuffer newState);
        void Write(BitBuffer buffer, BitBuffer previousState = null);
    }
}