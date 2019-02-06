using ImperialStudio.Core.Entities;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.Entities
{
    public sealed class TransformRotationState : VectorState
    {
        private readonly Transform m_Transform;

        public TransformRotationState(Transform transform)
        {
            m_Transform = transform;
        }

        protected override Vector3 GetCurrentValue()
        {
            return m_Transform.rotation.eulerAngles;
        }

        protected override void OnUpdateState(Vector3 oldValue, Vector3 newValue)
        {
            m_Transform.rotation = Quaternion.Euler(newValue);
        }
    }
}