using System.Numerics;
using ImperialStudio.Api.Entities;
using ImperialStudio.Core.Serialization;
using NetStack.Serialization;

namespace ImperialStudio.Core.Entities
{
    public abstract class VectorState : IEntityState
    {
        private Vector3 m_LastValue;

        public void WriteDelta(BitBuffer buffer, BitBuffer previousState)
        {
            Write(buffer);
            return;

            var newValue = GetCurrentValue();
            var deltaPosition = previousState.ReadVector3() - newValue;
            buffer.AddVector3(newValue);
        }

        public void ReadDelta(BitBuffer bitBuffer)
        {
            Read(bitBuffer);
            return;

            Vector3 delta = bitBuffer.ReadVector3();
            OnUpdateState(m_LastValue, (m_LastValue = m_LastValue + delta));
        }

        protected abstract Vector3 GetCurrentValue();
        protected abstract void OnUpdateState(Vector3 oldValue, Vector3 newValue);

        public int StateSize => 12;
        public void Write(BitBuffer buffer)
        {
            var value = GetCurrentValue();
            buffer.AddVector3(value);
        }

        public void Read(BitBuffer buffer)
        {
            var value = buffer.ReadVector3();
            OnUpdateState(m_LastValue, (m_LastValue = value));
        }
    }
}