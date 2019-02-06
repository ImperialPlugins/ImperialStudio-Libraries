using System;
using Castle.Windsor;
using ENet;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Scheduling;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Core.Logging;
using ImperialStudio.Networking.Events;
using ImperialStudio.Networking.Packets.Handlers;
using ILogger = ImperialStudio.Api.Logging.ILogger;

namespace ImperialStudio.Networking.Client
{
    public sealed class ClientConnectionHandler : BaseConnectionHandler
    {
        public ClientConnectionHandler(
            ITaskScheduler taskScheduler,
            IWindsorContainer container,
            IObjectSerializer packetSerializer,
            IIncomingNetworkEventHandler networkEventProcessor,
            ILogger logger,
            IEventBus eventBus,
            IGamePlatformAccessor gamePlatformAccessor) : base(packetSerializer, networkEventProcessor, logger, eventBus, gamePlatformAccessor)
        {
            m_TaskScheduler = taskScheduler;
            m_Container = container;
            m_Logger = logger;
            eventBus.Subscribe<NetworkEvent>(this, OnNetworkEvent);
        }

        private readonly ITaskScheduler m_TaskScheduler;
        private readonly IWindsorContainer m_Container;
        private readonly ILogger m_Logger;

        private bool m_ConnectingToServer;
        private Host m_Host;

        public NetworkPeer ServerPeer { get; private set; }
        public bool PendingTerminate { get; set; }

#if STEAM_AUTH
        private Auth.Ticket m_AuthTicket;
        private void SetSessionAuthTicket(Auth.Ticket ticket)
        {
            if (m_AuthTicket != null)
            {
                throw new Exception("An auth ticket is already active");
            }

            m_AuthTicket = ticket;
        }
#endif

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
#if STEAM_AUTH
            var clientId = SteamClientComponent.Instance.Client.SteamId;
            var ticket = SteamClientComponent.Instance.Client.Auth.GetAuthSessionTicket();
            var ticketData = ticket.Data;
            var userName = SteamClientComponent.Instance.Client.Username;
            SetSessionAuthTicket(ticket);
#else
            var clientId = 0u; // ignored
            var ticketData = Array.Empty<byte>();
            var userName = GenerateRandomName(10);
#endif
            Send(ServerPeer, new AuthenticatePacket
            {
                SteamId = clientId,
                Ticket = ticketData,
                Username = userName
            });
        }

        private string GenerateRandomName(int maxLen)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < r.Next(3, maxLen))
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }

        public override void Dispose()
        {
            ServerPeer.EnetPeer.DisconnectNow(0);

#if STEAM_AUTH
            m_AuthTicket?.Cancel();
            m_AuthTicket = null;
#endif

            base.Dispose();
        }

        private void OnNetworkEvent(object sender, NetworkEvent @event)
        {
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
