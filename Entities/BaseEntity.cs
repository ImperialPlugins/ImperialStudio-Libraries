using NetStack.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImperialStudio.Core.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public ushort Id { get; set; }
        public abstract string Name { get; protected set; }

        //Todo: change from ulong to NetworkPeer
        public ulong Owner { get; set; }
        public Transform Transform { get; protected set; }
        public bool IsDisposed { get; private set; }
        protected List<IEntityState> EntityStates { get; private set; }

        private bool m_Inited;
        public void Init()
        {
            if (m_Inited)
                return;

            EntityStates = new List<IEntityState>
            {
                new TransformPositionState(Transform),
                new TransformRotationState(Transform)
            };

            OnInit();
            m_Inited = true;
        }

        protected abstract void OnInit();

        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDispose();
            IsDisposed = true;
        }

        public int StateSize => EntityStates.Sum(d => d.StateSize + EntityStates.Count);

        public void Read(BitBuffer bitBuffer)
        {
            foreach (var state in EntityStates)
            {
                bool isDelta = bitBuffer.ReadBool();

                if (isDelta)
                    state.ReadDelta(bitBuffer);
                else
                    state.Read(bitBuffer);
            }
        }

        public void Write(BitBuffer buffer, BitBuffer previousState = null)
        {
            bool isDelta = previousState != null;

            foreach (var state in EntityStates)
            {
                buffer.AddBool(isDelta);

                if (isDelta)
                    state.WriteDelta(buffer, previousState);
                else
                    state.Write(buffer);
            }
        }

        protected abstract void OnDispose();
    }
}