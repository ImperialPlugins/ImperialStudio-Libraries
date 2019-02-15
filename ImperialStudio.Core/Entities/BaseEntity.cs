using System.Text;
using ImperialStudio.Api.Entities;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Networking;
using ImperialStudio.Networking.State;

namespace ImperialStudio.Core.Entities
{
    public abstract class BaseEntity : NetworkComponent, IEntity
    {
        private readonly IConnectionHandler m_ConnectionHandler;
        public int Id { get; set; }

        [NetworkState]
        public abstract string Name { get; set; }

        public uint? OwnerId { get; set; }
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

        protected BaseEntity(IObjectSerializer serializer, IConnectionHandler connectionHandler) : base(serializer)
        {
            m_ConnectionHandler = connectionHandler;
        }
    }
}