using ImperialStudio.Core.Entities;
using ImperialStudio.Core.UnityEngine.Math;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

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
            return m_Transform.rotation.eulerAngles.ToSystemVector();
        }

        protected override void OnUpdateState(Vector3 oldValue, Vector3 newValue)
        {
            m_Transform.rotation = global::UnityEngine.Quaternion.Euler(newValue.ToUnityVector());
        }
    }
}