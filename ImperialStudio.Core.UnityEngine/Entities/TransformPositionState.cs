using ImperialStudio.Core.Entities;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.Entities
{
    public sealed class TransformPositionState : VectorState
    {
        private readonly Transform m_Transform;

        public TransformPositionState(Transform transform)
        {
            m_Transform = transform;
        }

        protected override Vector3 GetCurrentValue()
        {
            return m_Transform.position;
        }

        protected override void OnUpdateState(Vector3 oldValue, Vector3 newValue)
        {
            m_Transform.position = newValue;
        }
    }
}