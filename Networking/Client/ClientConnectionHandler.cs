using Castle.Windsor;
using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets.Handlers;
using ImperialStudio.Core.Steam;
using System;
using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Map;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Scheduling;
using ImperialStudio.Core.Api.Serialization;
using ImperialStudio.Core.Map;
using ImperialStudio.Core.Scheduling;
using ImperialStudio.Core.Serialization;
using ILogger = ImperialStudio.Core.Api.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Client
{
    public sealed class ClientConnectionHandler : BaseConnectionHandler
    {
        public ClientConnectionHandler(
            IObjectSerializer packetSerializer, 
            Api.Networking.IIncomingNetworkEventHandler networkEventProcessor, 
            ILogger logger, 
            IEventBus eventBus, 
            ITaskScheduler scheduler,
            IMapManager mapManager) : base(packetSerializer, networkEventProcessor, logger, eventBus)
        {
            m_Logger = logger;
            m_Scheduler = scheduler;
            m_MapManager = mapManager;
            eventBus.Subscribe<NetworkEvent>(this, OnNetworkEvent);
        }

        private readonly ILogger m_Logger;
        private readonly ITaskScheduler m_Scheduler;
        private readonly IMapManager m_MapManager;

        private Auth.Ticket m_AuthTicket;
        private bool m_ConnectingToServer;
        private Host m_Host;

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

            m_Host = GetOrCreateHost();
            m_Host.Create();

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

        private void OnNetworkEvent(object sender, NetworkEvent @event)
        {
            if (@event.EnetEvent.Type == EventType.Disconnect || @event.EnetEvent.Type == EventType.Timeout)
            {
                if (!PendingTerminate)
                {
                    m_Scheduler.ScheduleUpdate(this, ()=> m_MapManager.GoToMainMenu(), "HandleNetworkEvent->GoToMainMenu", ExecutionTargetContext.NextFrame);
                }

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

                m_Logger.LogInformation($"Connected to server: {ServerPeer.Ip}:{ServerPeer.Port}");
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
