using ImperialStudio.Api.Entities;
using ImperialStudio.Core.Entities;
using ImperialStudio.Core.UnityEngine.Math;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace ImperialStudio.Core.UnityEngine.Entities
{
    public abstract class UnityGameObjectEntity : BaseEntity, IWorldEntity
    {
        public abstract GameObject GameObject { get; }

        protected override void OnInit()
        {
            EntityStates.Add(new TransformPositionState(GameObject.transform));
            EntityStates.Add(new TransformRotationState(GameObject.transform));
        }

        protected override void OnDispose()
        {
        }

        public Vector3 Position
        {
            get => GameObject.transform.position.ToSystemVector();
            set => GameObject.transform.position = value.ToUnityVector();
        }

        public Vector3 Rotation
        {
            get => GameObject.transform.rotation.eulerAngles.ToSystemVector();
            set => GameObject.transform.rotation = Quaternion.Euler(value.ToUnityVector());
        }
    }
}