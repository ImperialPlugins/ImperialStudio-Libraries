using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Castle.Windsor;
using Disruptor;
using ENet;
using ImperialStudio.Api.Eventing;
using ImperialStudio.Api.Game;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Networking.Events;
using ImperialStudio.Networking.Packets;
using ImperialStudio.Networking.Server;
using Event = ENet.Event;
using EventType = ENet.EventType;
using ILogger = ImperialStudio.Api.Logging.ILogger;

namespace ImperialStudio.Networking
{
    public abstract class BaseConnectionHandler : IConnectionHandler
    {
        public const byte ChannelUpperLimit = 255;

        private readonly RingBuffer<OutgoingPacket> m_OutgoingQueue;
        private readonly ICollection<INetworkPeer> m_ConnectedPeers;
        private readonly IDictionary<byte, PacketDescription> m_PacketDescriptions;

        public bool IsListening { get; private set; }

        private Host m_Host;
        private Thread m_NetworkThread;
        private volatile bool m_Flush;

        public bool AutoFlush { get; set; } = true;

        private readonly IObjectSerializer m_PacketSerializer;
        private readonly IIncomingNetworkEventHandler m_NetworkEventProcessor;
        private readonly ILogger m_Logger;
        private readonly IEventBus m_EventBus;
        private readonly IWindsorContainer m_Container;

        protected BaseConnectionHandler(IObjectSerializer packetSerializer, IIncomingNetworkEventHandler networkEventProcessor, ILogger logger, IEventBus eventBus, IGamePlatformAccessor gamePlatformAccessor)
        {
            eventBus.Subscribe<GameQuitEvent>(this, (s, e) => { Dispose(); });

            m_EventBus = eventBus;
            m_PacketSerializer = packetSerializer;
            m_NetworkEventProcessor = networkEventProcessor;
            m_Logger = logger;

            int capacity = 1024;
            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
                capacity *= ServerConnectionHandler.MaxPlayersUpperLimit;
            }

            m_OutgoingQueue = new RingBuffer<OutgoingPacket>(capacity);
            m_ConnectedPeers = new List<INetworkPeer>();
            m_PacketDescriptions = new Dictionary<byte, PacketDescription>();

            RegisterPacket((byte)PacketType.Ping, new PacketDescription(
                name: nameof(PacketType.Ping),
                direction: PacketDirection.Any,
                channel: (byte)NetworkChannel.PingPong,
                packetFlags: PacketFlags.Reliable)
            );

            RegisterPacket((byte)PacketType.Pong, new PacketDescription(
                name: nameof(PacketType.Pong),
                direction: PacketDirection.Any,
                channel: (byte)NetworkChannel.PingPong,
                packetFlags: PacketFlags.Reliable)
            );

            RegisterPacket((byte)PacketType.Authenticate, new PacketDescription(
                name: nameof(PacketType.Authenticate),
                direction: PacketDirection.ClientToServer,
                channel: (byte)NetworkChannel.Main,
                packetFlags: PacketFlags.Reliable,
                needsAuthentication: false)
            );

            RegisterPacket((byte)PacketType.Authenticated, new PacketDescription(
                name: nameof(PacketType.Authenticated),
                direction: PacketDirection.ServerToClient,
                channel: (byte)NetworkChannel.Main,
                packetFlags: PacketFlags.Reliable)
            );

            RegisterPacket((byte)PacketType.MapChange, new PacketDescription(
                name: nameof(PacketType.MapChange),
                direction: PacketDirection.ServerToClient,
                channel: (byte)NetworkChannel.Main,
                packetFlags: PacketFlags.Reliable)
            );

            RegisterPacket((byte)PacketType.WorldUpdate, new PacketDescription(
                name: nameof(PacketType.WorldUpdate),
                direction: PacketDirection.ServerToClient,
                channel: (byte)NetworkChannel.World,
                packetFlags: PacketFlags.Reliable) //Todo: Make this Unsequenced after Snapshots get implemented fully
            );

            RegisterPacket((byte)PacketType.InputUpdate, new PacketDescription(
                name: nameof(PacketType.InputUpdate),
                direction: PacketDirection.ClientToServer,
                channel: (byte)NetworkChannel.Input,
                packetFlags: PacketFlags.Unsequenced)
            );

            RegisterPacket((byte)PacketType.Terminate, new PacketDescription(
                name: nameof(PacketType.Terminate),
                direction: PacketDirection.ServerToClient,
                channel: (byte)NetworkChannel.Input,
                packetFlags: PacketFlags.Reliable)
            );
        }

        public void Send<T>(INetworkPeer peer, T packet) where T : class, IPacket
        {
            Send<T>(peer, packet, packet.GetPacketId());
        }

        public void Send<T>(INetworkPeer peer, T packet, byte packetId) where T : class, IPacket
        {
            byte[] data = m_PacketSerializer.Serialize(packet);

            Send(new OutgoingPacket
            {
                Data = data,
                PacketId = packetId,
                Peers = new[] { peer }
            });
        }

        public void Send(OutgoingPacket packet)
        {
            m_OutgoingQueue.Enqueue(packet);
        }

        public void Flush()
        {
            m_Flush = true;
        }

        private void NetworkUpdate()
        {
            while (IsListening)
            {
                if (m_Host == null)
                {
                    break;
                }

                lock (m_Host)
                {
                    m_Host.Service(1, out var netEvent);
                    if (netEvent.Type != EventType.None)
                    {
#if LOG_NETWORK
                        m_Logger.LogDebug("Network event: " + netEvent.Type);
#endif

                        HandleIncomingEvent(netEvent);
                    }

                    while (m_OutgoingQueue.Count > 0)
                    {
                        var outgoingPacket = m_OutgoingQueue.Dequeue();
                        HandleOutgoingPacket(outgoingPacket);
                    }

                    if (AutoFlush || m_Flush)
                    {
                        m_Host?.Flush();
                        m_Flush = false;
                    }
                }
            }
        }

        private void HandleIncomingEvent(Event @event)
        {
#if LOG_NETWORK
            m_Logger.LogDebug("Processing network event: " + @event.Type);
#endif
            INetworkPeer networkPeer = null;
            if (@event.Peer.IsSet)
            {
                networkPeer = GetPeerByNetworkId(@event.Peer.ID, false);
            }

            switch (@event.Type)
            {
                case EventType.Connect:
                    RegisterPeer(new NetworkPeer(@event.Peer));
                    break;
                case EventType.Timeout:
                case EventType.Disconnect:
                    UnregisterPeer(networkPeer);
                    break;
            }

            ENetNetworkEvent enetEvent = new ENetNetworkEvent(@event, networkPeer);
            m_EventBus.Emit(this, enetEvent);

            if (enetEvent.IsCancelled)
                return;

            m_NetworkEventProcessor.ProcessNetworkEvent(@event, networkPeer);
        }

        private void HandleOutgoingPacket(OutgoingPacket outgoingPacket)
        {
            var packetDescription = GetPacketDescription(outgoingPacket.PacketId);

#if LOG_NETWORK
            m_Logger.LogDebug($"[Network] > {packetDescription.Name}");
#endif
            var enetPeers = outgoingPacket.Peers.Select(d => ((NetworkPeer)d).EnetPeer).ToArray();

            Packet packet = default;

            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)outgoingPacket.PacketId);
            ms.Write(outgoingPacket.Data.ToArray(), 0, outgoingPacket.Data.Length);

            packet.Create(ms.ToArray(), packetDescription.PacketFlags);
            ms.Dispose();

            var channelId = packetDescription.ChannelId;
            lock (m_Host)
            {
                if (enetPeers.Length == 1)
                    enetPeers.First().Send(channelId, ref packet);
                else

                    m_Host.Broadcast(channelId, ref packet, enetPeers);
            }
        }

        protected Host GetOrCreateHost()
        {
            return m_Host ?? (m_Host = new Host());
        }

        protected void StartListening()
        {
            if (IsListening)
            {
                throw new Exception("Network thread is already listening!");
            }

            IsListening = true;

            m_NetworkEventProcessor.EnsureLoaded();

            m_NetworkThread = new Thread(NetworkUpdate)
            {
                Name = "NetworkThread-" + GetType().Name
            };
            m_NetworkThread.Start();
        }

        protected void StopListening()
        {
            IsListening = false;
        }

        public virtual void Dispose()
        {
            if (!IsListening)
            {
                return;
            }

            IsListening = false;

            lock (m_Host)
            {
                m_Host?.Dispose();
                m_Host = null;
            }
        }

        public IEnumerable<INetworkPeer> GetPeers(bool authenticatedOnly = true)
        {
            IEnumerable<INetworkPeer> peers = m_ConnectedPeers;
            if (authenticatedOnly)
                peers = peers.Where(d => d.IsAuthenticated);

            return peers;
        }

        public IEnumerable<INetworkPeer> GetPendingPeers()
        {
            return m_ConnectedPeers.Where(d => !d.IsAuthenticated);
        }

        public void RegisterPeer(INetworkPeer networkPeer)
        {
            m_ConnectedPeers.Add(networkPeer);
        }

        public void UnregisterPeer(INetworkPeer networkPeer)
        {
            m_ConnectedPeers.Remove(networkPeer);
        }

        public void RegisterPacket(byte id, PacketDescription packetDescription)
        {
            if (m_PacketDescriptions.ContainsKey(id))
            {
                throw new Exception($"Packet with id: {id} is already registered!");
            }

            m_PacketDescriptions.Add(id, packetDescription);
        }

        public PacketDescription GetPacketDescription(byte id)
        {
            if (!m_PacketDescriptions.ContainsKey(id))
            {
                throw new Exception($"Packet is not registered: {id}");
            }

            return m_PacketDescriptions[id];
        }

        public INetworkPeer GetPeerByNetworkId(uint peerID, bool authenticatedOnly = true)
        {
            return GetPeers(authenticatedOnly).FirstOrDefault(d => d.Id == peerID);
        }

        public INetworkPeer GetPeerBySteamId(ulong steamId, bool authenticatedOnly = true)
        {
            return GetPeers(authenticatedOnly).FirstOrDefault(d => d.SteamId == steamId);
        }
    }
}