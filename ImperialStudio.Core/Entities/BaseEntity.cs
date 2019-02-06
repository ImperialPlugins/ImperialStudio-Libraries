using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Networking;
using NetStack.Serialization;
using System.Collections.Generic;

namespace ImperialStudio.Core.Entities
{
    public abstract class BaseEntity : IEntity
    {
        public int Id { get; set; }
        public virtual string Name { get; set; }

        public INetworkPeer Owner { get; set; }
        public bool IsDisposed { get; private set; }
        protected List<IEntityState> EntityStates { get; private set; }

        private bool m_Inited;
        public void Init()
        {
            if (m_Inited)
                return;

            Name = $"Entity[{GetType().Name.Replace("Entity", "")}]-{Id}";
            if (Owner?.Username != null)
                Name += $"({Owner.Username})";

            EntityStates = new List<IEntityState>();
            EntityStates.Add(new EntityNameState(this));

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

        public bool IsOwner { get; set; }

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