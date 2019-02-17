using System.Text;
using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Networking;
using ImperialStudio.Networking.Components;
using ImperialStudio.Networking.Packets;
using ImperialStudio.Networking.Rpc;
using ImperialStudio.Networking.State;

namespace ImperialStudio.Core.Entities
{
    public abstract class BaseEntity : NetworkComponent, IEntity
    {
        private readonly IConnectionHandler m_ConnectionHandler;

        private int m_Id;

        public int Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                ComponentId = m_Id;
                m_Id = value;
            }
        }

        [NetworkVariable]
        public abstract string Name { get; set; }

        private uint? m_OwnerId;
        public uint? OwnerId
        {
            get
            {
                return m_OwnerId;
            }
            set
            {
                if (m_OwnerId == value)
                {
                    return;
                }

                if (m_OwnerId != null)
                {
                    // Notify old owner
                    InvokeRpc(nameof(Rpc_SetOwner), m_OwnerId.Value, false);
                }

                if (value != null)
                {
                    // Notify new owner
                    InvokeRpc(nameof(Rpc_SetOwner), value.Value, true);
                }

                m_OwnerId = value;
            }
        }

        public bool IsDisposed { get; private set; }

        private bool m_Inited;
        public void Init()
        {
            if (m_Inited)
                return;

            StringBuilder nameBuilder = new StringBuilder();
            nameBuilder.Append("Entity[");
            nameBuilder.Append(GetType().Name.Replace("Entity", ""));
            nameBuilder.Append("]-");
            nameBuilder.Append(Id);

            if (OwnerId != null)
            {
                var owner = m_ConnectionHandler.GetPeerByNetworkId(OwnerId.Value);
                if (owner?.Username != null)
                {
                    nameBuilder.Append("(");
                    nameBuilder.Append(owner.Username);
                    nameBuilder.Append(")");
                }
            }

            Name = nameBuilder.ToString();

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

        protected abstract void OnDispose();

        protected BaseEntity(IObjectSerializer serializer, IConnectionHandler connectionHandler, INetworkComponentManager networkComponentManager) : base(serializer, connectionHandler, networkComponentManager)
        {
            m_ConnectionHandler = connectionHandler;
        }

        [Rpc(PacketDirection.ServerToClient)]
        public void Rpc_SetOwner(INetworkPeer sender, bool isOwner)
        {

        }
    }
}