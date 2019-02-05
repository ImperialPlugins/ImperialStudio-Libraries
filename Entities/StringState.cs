using ImperialStudio.Core.Api.Entities;
using NetStack.Serialization;

namespace ImperialStudio.Core.Entities
{
    public abstract class StringState : IEntityState
    {
        private string m_CachedValue;

        public void Read(BitBuffer buffer)
        {
            bool changed = buffer.ReadByte() == 1;
            if (!changed)
            {
                return;
            }

            string newName = buffer.ReadString();
            OnUpdateState(m_CachedValue, newName);
        }

        public void ReadDelta(BitBuffer bitBuffer)
        {
            Read(bitBuffer);
        }

        public void Write(BitBuffer buffer)
        {
            string currentName = GetCurrentValue();

            if (m_CachedValue == currentName)
            {
                buffer.AddByte(0);
                return;
            }

            buffer.AddByte(1);
            buffer.AddString(currentName);
            m_CachedValue = currentName;
        }

        protected abstract string GetCurrentValue();
        protected abstract void OnUpdateState(string oldValue, string newValue);

        public void WriteDelta(BitBuffer buffer, BitBuffer previousState)
        {
            Write(buffer);
        }
    }
}