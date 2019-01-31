using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Entities;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Map;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets.Handlers;
using ImperialStudio.Core.Serialization;
using ImperialStudio.Core.Util;
using NetStack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using EventType = ENet.EventType;
using ILogger = ImperialStudio.Core.Logging.ILogger;

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
            INetworkEventHandler networkEventProcessor,
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

        public void Disconnect(NetworkPeer peer, ServerAuth.Status reason)
        {
            if (peer.EnetPeer.State == PeerState.Disconnected || peer.EnetPeer.State == PeerState.Disconnecting || peer.EnetPeer.State == PeerState.DisconnectLater)
            {
                return;
            }

            m_Logger.LogWarning($"{peer.Name} was rejected because of failed Steam authentication: {reason}.");

            Send(peer, new TerminatePacket
            {
                AuthFailureReason = reason
            });
            peer.EnetPeer.Disconnect(0);
        }

        public void Disconnect(NetworkPeer peer, string reason)
        {
            if (peer.EnetPeer.State == PeerState.Disconnected || peer.EnetPeer.State == PeerState.Disconnecting || peer.EnetPeer.State == PeerState.DisconnectLater)
            {
                return;
            }

            m_Logger.LogInformation(peer.Name + " was disconnected: " + reason);

            Send(peer, new TerminatePacket
            {
                Reason = reason
            });
            peer.EnetPeer.Disconnect(0);
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
                OnWorldUpdate();

                Thread.Sleep(WorldUpdateInterval);
            }
        }

        private void OnWorldUpdate()
        {
            using (new SpeedBenchmark("OnWorldUpdate", m_Logger))
            {
                Dictionary<ushort, byte[]> entityFullStates = new Dictionary<ushort, byte[]>();
                var entities = m_EntityManager.GetEntities().ToList();

                foreach (var entity in entities)
                {
                    BitBuffer buffer = new BitBuffer();
                    entity.Write(buffer);

                    byte[] state = new byte[entity.StateSize];
                    buffer.ToArray(state);

                    entityFullStates.Add(entity.Id, state);
                }


                foreach (var peer in GetPeers())
                {
                    Dictionary<ushort, byte[]> states = new Dictionary<ushort, byte[]>();
                    List<WorldSpawn> spawns = new List<WorldSpawn>(); 

                    foreach (var entity in entities)
                    {
                        byte[] stateToSend = new byte[entity.StateSize];
                        byte[] fullState = entityFullStates[entity.Id];

                        if (!m_Snapshots.HasSnapshot(peer, entity.Id))
                        {
                            spawns.Add(new WorldSpawn(entity));
                            stateToSend = fullState;
                        }
                        else
                        {
                            byte[] previousState = m_Snapshots.GetSnapshot(peer, entity.Id);

                            var previousStateBuffer = new BitBuffer(previousState.Length);
                            previousStateBuffer.AddBytes(previousState);
                            
                            BitBuffer sendBuffer = new BitBuffer(entity.StateSize);
                            entity.Write(sendBuffer, previousStateBuffer);
                            sendBuffer.ToArray(stateToSend);
                        }

                        states.Add(entity.Id, stateToSend);
                        m_Snapshots.AddSnapshot(peer, entity.Id, fullState);
                    }

                    WorldUpdatePacket packet = new WorldUpdatePacket
                    {
                        Spawns = spawns,
                        EntityStates = states,
                        Despawns = new ushort[0] // todo
                    };

                    Send(peer, packet);
                }
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

                    bool isConnected = pendingPing.NetworkPeer.EnetPeer.State == PeerState.Connected;

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
                    m_Logger.LogDebug($"{peer.Name} ping: {peer.EnetPeer.RoundTripTime}ms");
                }
#endif
            }

            m_LastPing = DateTime.UtcNow;
        }

        private void OnPong(object sender, PongEvent @event)
        {
            var pendingPing = m_PendingPings.FirstOrDefault(d => d.PingId == @event.PongPacket.PingId && d.NetworkPeer.EnetPeer.ID == @event.Peer.EnetPeer.ID);
            if (pendingPing.PingId == 0)
                return;

            m_PendingPings.Remove(pendingPing);

            TimeSpan ping = DateTime.UtcNow - pendingPing.SendTime;
            @event.Peer.Ping = ping;

#if LOG_NETWORK
            m_Logger.LogDebug($"{@event.Peer.Name} ping: {ping.TotalMilliseconds}ms");
#endif
        }

        private struct PendingPing
        {
            public NetworkPeer NetworkPeer { get; set; }
            public DateTime SendTime { get; set; }
            public ulong PingId { get; set; }
        }
    }
}
