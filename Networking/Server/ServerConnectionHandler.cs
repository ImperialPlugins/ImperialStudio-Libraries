using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets.Handlers;
using System;
using System.Linq;
using System.Threading;
using ImperialStudio.Core.Api.Entities;
using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Map;
using ImperialStudio.Core.Api.Networking;
using ImperialStudio.Core.Api.Serialization;
using UnityEngine;
using EventType = ENet.EventType;
using ILogger = ImperialStudio.Core.Api.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Server
{
    public sealed class ServerConnectionHandler : BaseConnectionHandler
    {
        public const byte MaxPlayersUpperLimit = byte.MaxValue;
        public TimeSpan ClientTimeOut { get; set; }
        public string Name { get; private set; }
        public byte MaxPlayers { get; set; }
        public bool UseCustomPingHandler { get; set; } = true;

        public ServerConnectionHandler(
            IObjectSerializer packetSerializer,
            Api.Networking.IIncomingNetworkEventHandler networkEventProcessor,
            ILogger logger,
            IEntityManager entityManager,
            IEventBus eventBus,
            ISnapshotCache snapshotCache,
            IMapManager mapManager) : base(packetSerializer, networkEventProcessor, logger, eventBus)
        {
            m_EventBus = eventBus;
            m_MapManager = mapManager;
            m_Logger = logger;
            m_EntityManager = entityManager;

            m_Snapshots = snapshotCache;
            ClientTimeOut = TimeSpan.FromSeconds(15);
        }

        private const int PingInterval = 1000 * 5;
        private const int WorldUpdateInterval = 50;

        private readonly IEventBus m_EventBus;
        private readonly IMapManager m_MapManager;
        private readonly ILogger m_Logger;
        private readonly IEntityManager m_EntityManager;
        private readonly ISnapshotCache m_Snapshots;

        private Thread m_ServerThread;

        public void Host(ServerListenParameters listenParameters)
        {
            Name = listenParameters.Name;
            MaxPlayers = listenParameters.MaxPlayers;

            if (IsListening)
            {
                throw new Exception("Server is already listening.");
            }

            string hostName = listenParameters.Host;
            ushort port = listenParameters.Port;

            Address address = new Address { Port = port };

            if (!address.SetHost(hostName))
            {
                throw new Exception($"Failed to resolve host: \"{hostName}\"");
            }

            var host = GetOrCreateHost();

            try
            {
                host.Create(address, MaxPlayersUpperLimit, ChannelUpperLimit);
            }
            catch (Exception ex)
            {
                m_Logger.LogError($"Failed to host server. Is port {listenParameters.Port} already being used?", ex);
                Application.Quit(1);
                return;
            }

            StartListening();

            m_Logger.LogInformation($"Hosted server: {hostName}:{listenParameters.Port}");
            StartPingThread();

            m_MapManager.ChangeMap(listenParameters.Map);

            m_EventBus.Emit(this, new ServerInitializedEvent(listenParameters));
            m_EventBus.Subscribe<NetworkEvent>(this, OnNetworkEvent);
        }

        private void OnNetworkEvent(object sender, NetworkEvent @event)
        {
            if (@event.EnetEvent.Type == EventType.Disconnect || @event.EnetEvent.Type == EventType.Timeout)
            {
                m_Snapshots.Clear(@event.NetworkPeer);
            }
        }

        public void Disconnect(INetworkPeer peer, ServerAuth.Status reason)
        {
            var enetPeer = ((NetworkPeer) peer).EnetPeer;

            if (enetPeer.State == PeerState.Disconnected || enetPeer.State == PeerState.Disconnecting || enetPeer.State == PeerState.DisconnectLater)
            {
                return;
            }

            m_Logger.LogWarning($"{peer} was rejected because of failed Steam authentication: {reason}.");

            Send(peer, new TerminatePacket
            {
                AuthFailureReason = reason
            });
            enetPeer.Disconnect(0);
        }

        public void Disconnect(INetworkPeer peer, string reason)
        {
            var enetPeer = ((NetworkPeer)peer).EnetPeer;

            if (enetPeer.State == PeerState.Disconnected || enetPeer.State == PeerState.Disconnecting || enetPeer.State == PeerState.DisconnectLater)
            {
                return;
            }

            m_Logger.LogInformation($"{peer} was disconnected: {reason}");

            Send(peer, new TerminatePacket
            {
                Reason = reason
            });
            enetPeer.Disconnect(0);
        }

        private void StartPingThread()
        {
            m_PendingPings = new Boo.Lang.List<PendingPing>();

            m_ServerThread = new Thread(ServerTick);
            m_ServerThread.Start();
        }

        private Boo.Lang.List<PendingPing> m_PendingPings = new Boo.Lang.List<PendingPing>();
        private ulong m_NextPingId = 1;

        private void ServerTick()
        {
            m_EventBus.Subscribe<PongEvent>(this, OnPong);

            while (IsListening)
            {
                PingClients();

                Thread.Sleep(WorldUpdateInterval);
            }
        }

      

        private DateTime m_LastPing;
        private void PingClients()
        {
            if ((DateTime.UtcNow - m_LastPing).TotalMilliseconds > PingInterval)
                return;

            if (UseCustomPingHandler)
            {
                var toDisconnect = new Boo.Lang.List<PendingPing>();

                foreach (var pendingPing in m_PendingPings)
                {
                    TimeSpan ping = DateTime.UtcNow - pendingPing.SendTime;

                    if (ping > ClientTimeOut)
                        toDisconnect.Add(pendingPing);

                    bool isConnected = ((NetworkPeer)pendingPing.NetworkPeer).EnetPeer.State == PeerState.Connected;

                    if (isConnected)
                    {
                        Disconnect(pendingPing.NetworkPeer, "Timeout.");
                    }
                }

                m_PendingPings.RemoveAll(d => toDisconnect.Contains(d));

                foreach (var client in GetPeers())
                {
                    ulong currentPingId = m_NextPingId;

                    PendingPing pendingPing = new PendingPing
                    {
                        SendTime = DateTime.UtcNow,
                        NetworkPeer = client,
                        PingId = currentPingId
                    };

                    m_PendingPings.Add(pendingPing);

                    Send(client, new PingPacket
                    {
                        PingId = currentPingId
                    });

                    m_NextPingId++;
                }

            }
            else
            {
#if LOG_NETWORK
                foreach (var peer in GetPeers())
                {
                    var enetPeer = ((NetworkPeer) peer).EnetPeer;
                    m_Logger.LogDebug($"{peer} ping: {enetPeer.RoundTripTime}ms");
                }
#endif
            }

            m_LastPing = DateTime.UtcNow;
        }

        private void OnPong(object sender, PongEvent @event)
        {
            var pendingPing = m_PendingPings.FirstOrDefault(d => d.PingId == @event.PongPacket.PingId && d.NetworkPeer.Id == @event.Peer.Id);
            if (pendingPing.PingId == 0)
                return;

            m_PendingPings.Remove(pendingPing);

            TimeSpan ping = DateTime.UtcNow - pendingPing.SendTime;
            @event.Peer.Ping = ping;

#if LOG_NETWORK
            m_Logger.LogDebug($"{@event.Peer} ping: {ping.TotalMilliseconds}ms");
#endif
        }

        private struct PendingPing
        {
            public INetworkPeer NetworkPeer { get; set; }
            public DateTime SendTime { get; set; }
            public ulong PingId { get; set; }
        }
    }
}
