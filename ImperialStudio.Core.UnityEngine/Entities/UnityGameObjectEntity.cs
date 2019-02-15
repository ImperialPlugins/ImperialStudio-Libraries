using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Core.Entities;
using ImperialStudio.Core.UnityEngine.Math;
using ImperialStudio.Networking;
using ImperialStudio.Networking.State;
using UnityEngine;
using SVector3 = ImperialStudio.Core.Math.SVector3;
using Vector3 = System.Numerics.Vector3;

namespace ImperialStudio.Core.UnityEngine.Entities
{
    public abstract class UnityGameObjectEntity : BaseEntity, IWorldEntity
    {
        public abstract GameObject GameObject { get; }

        [NetworkState]
        private SVector3? SyncRotation
        {
            get
            {
                var rot = GameObject?.transform.rotation.eulerAngles;

                if (rot == null)
                    return null;

                return new SVector3(rot.Value.x, rot.Value.y, rot.Value.z);
            }
            set
            {
                if (value != null && GameObject?.transform != null)
                {
                    GameObject.transform.rotation = Quaternion.Euler(value.Value.X, value.Value.Y, value.Value.Z);
                }
            }
        }

        [NetworkState]
        private SVector3? SyncPosition
        {
            get
            {
                var pos = GameObject?.transform.position;
                if (pos == null)
                    return null;

                return new SVector3(pos.Value.x, pos.Value.y, pos.Value.z);
            } 
            set
            {
                if (value != null && GameObject?.transform != null)
                {
                    GameObject.transform.position = new global::UnityEngine.Vector3(value.Value.X, value.Value.Y, value.Value.Z);
                }
            }
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

        protected UnityGameObjectEntity(IObjectSerializer serializer, IConnectionHandler connectionHandler) : base(serializer, connectionHandler)
        {

        }
    }
}