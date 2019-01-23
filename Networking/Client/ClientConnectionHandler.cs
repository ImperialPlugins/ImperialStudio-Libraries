using System;
using Castle.Windsor;
using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Packets.Handlers;
using ImperialStudio.Core.Networking.Packets.Serialization;
using ImperialStudio.Core.Steam;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Client
{
    public sealed class ClientConnectionHandler : BaseConnectionHandler
    {
        public ClientConnectionHandler(IPacketSerializer packetSerializer, ILogger logger, IEventBus eventBus, IWindsorContainer container) : base(packetSerializer, logger, eventBus, container)
        {
            m_Logger = logger;
        }

        private readonly ILogger m_Logger;
        private Auth.Ticket m_AuthTicket;

        public Peer ServerPeer { get; private set; }

        private void SetSessionAuthTicket(Auth.Ticket ticket)
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

            InitializeAuthentication();
        }

        private void InitializeAuthentication()
        {
            var clientId = SteamClientComponent.Instance.Client.SteamId;
            var ticket = SteamClientComponent.Instance.Client.Auth.GetAuthSessionTicket();

            SetSessionAuthTicket(ticket);
            var packet = new AuthenticatePacket
            {
                SteamId = clientId,
                Ticket = ticket.Data
            };

            Send(ServerPeer, packet);
        }

        public override void Dispose()
        {
            base.Dispose();

            m_AuthTicket?.Cancel();
            m_AuthTicket = null;
        }
    }
}
