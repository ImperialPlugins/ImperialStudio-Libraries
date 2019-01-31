using Castle.Windsor;
using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets.Handlers;
using ImperialStudio.Core.Steam;
using System;
using ImperialStudio.Core.Map;
using ImperialStudio.Core.Serialization;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Client
{
    public sealed class ClientConnectionHandler : BaseConnectionHandler
    {
        public ClientConnectionHandler(
            IObjectSerializer packetSerializer, 
            INetworkEventHandler networkEventProcessor, 
            ILogger logger, 
            IEventBus eventBus, 
            IMapManager mMapManager) : base(packetSerializer, networkEventProcessor, logger, eventBus)
        {
            m_Logger = logger;
            m_MapManager = mMapManager;
            eventBus.Subscribe<NetworkEvent>(this, HandleNetworkEvent);
        }

        private readonly ILogger m_Logger;
        private readonly IMapManager m_MapManager;

        private Auth.Ticket m_AuthTicket;
        private bool m_ConnectingToServer;
        public NetworkPeer ServerPeer { get; private set; }
        public bool PendingTerminate { get; set; }

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

            m_ConnectingToServer = true;
            var serverPeer = m_Host.Connect(address, ChannelUpperLimit);

            ServerPeer = new NetworkPeer(serverPeer) { IsAuthenticated = true };
            RegisterPeer(ServerPeer);
        }

        private void InitializeAuthentication()
        {
            var clientId = SteamClientComponent.Instance.Client.SteamId;
            var ticket = SteamClientComponent.Instance.Client.Auth.GetAuthSessionTicket();
            var userName = SteamClientComponent.Instance.Client.Username;
            SetSessionAuthTicket(ticket);

            Send(ServerPeer, new AuthenticatePacket
            {
                SteamId = clientId,
                Ticket = ticket.Data,
                Username = userName
            });
        }

        public override void Dispose()
        {
            base.Dispose();

            m_AuthTicket?.Cancel();
            m_AuthTicket = null;
        }

        private void HandleNetworkEvent(object sender, NetworkEvent @event)
        {
            if (@event.EnetEvent.Type == EventType.Disconnect)
            {
                if (!PendingTerminate)
                    m_MapManager.GoToMainMenu();

                PendingTerminate = true;
            }

            if (@event.EnetEvent.Type == EventType.Connect)
            {
                if (!m_ConnectingToServer)
                    return; //handle other peers

                if (ServerPeer == null)
                {
                    throw new Exception("Failed to get server peer");
                }

                m_Logger.LogInformation($"Connected to server: {ServerPeer.EnetPeer.IP}:{ServerPeer.EnetPeer.Port}");
                m_Host.PreventConnections(true);

                InitializeAuthentication();
                m_ConnectingToServer = false;
            }
        }

        public void Disconnect()
        {
            ServerPeer.EnetPeer.DisconnectNow(0);
            StopListening();
        }
    }
}
