using System;
using Castle.Windsor;
using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Steam;
using Unity.Collections.LowLevel.Unsafe;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Client
{
    public sealed class ClientConnectionHandler : BaseConnectionHandler
    {
        public ClientConnectionHandler(ILogger logger, IEventBus eventBus, IWindsorContainer container) : base(logger, eventBus, container)
        {
            m_Logger = logger;
        }

        private readonly ILogger m_Logger;
        private Auth.Ticket m_AuthTicket;

        public Peer ServerPeer { get; private set; }

        public void SetSessionAuthTicket(Auth.Ticket ticket)
        {
            if (m_AuthTicket != null)
            {
                throw new Exception("An auth ticket is already active");
            }

            m_AuthTicket = ticket;
        }

        public void Connect(ClientConnectParameters connectParameters)
        {
            Address address = new Address { Port = connectParameters.Port };
            if (!address.SetHost(connectParameters.Host))
            {
                throw new Exception("Failed to resolve host: " + connectParameters.Host);
            }

            Connect(address);
        }

        public void Connect(Address address)
        {
            if (IsListening)
            {
                throw new Exception("Client is already connected.");
            }

            var host = GetOrCreateHost();
            host.Create();

            StartListening();
            ServerPeer = m_Host.Connect(address);
            m_Logger.LogInformation($"Connecting to server: {address.GetHost()}:{address.Port}");
        }

        public override void Dispose()
        {
            base.Dispose();
            m_AuthTicket?.Cancel();
        }
    }
}
