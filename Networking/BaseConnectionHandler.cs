using Castle.Windsor;
using ENet;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Events;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets;
using ImperialStudio.Core.Networking.Packets.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ImperialStudio.Core.Serialization;
using Event = ENet.Event;
using EventType = ENet.EventType;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Networking
{
    public abstract class BaseConnectionHandler : IConnectionHandler
    {
        public const byte ChannelUpperLimit = 255;

        private readonly Queue<OutgoingPacket> m_OutgoingQueue;
        private readonly ICollection<NetworkPeer> m_ConnectedPeers;

        public bool IsListening { get; private set; }
        protected Host m_Host { get; private set; }

        private Thread m_NetworkThread;
        private volatile bool m_Flush;

        public bool AutoFlush { get; set; } = true;

        private readonly IObjectSerializer m_PacketSerializer;
        private readonly INetworkEventHandler m_NetworkEventProcessor;
        private readonly ILogger m_Logger;
        private readonly IEventBus m_EventBus;
        private readonly IWindsorContainer m_Container;

        protected BaseConnectionHandler(IObjectSerializer packetSerializer, INetworkEventHandler networkEventProcessor, ILogger logger, IEventBus eventBus)
        {
            eventBus.Subscribe<ApplicationQuitEvent>(this, (s, e) => { Dispose(); });

            m_EventBus = eventBus;
            m_PacketSerializer = packetSerializer;
            m_NetworkEventProcessor = networkEventProcessor;
            m_Logger = logger;
            m_OutgoingQueue = new Queue<OutgoingPacket>();
            m_ConnectedPeers = new List<NetworkPeer>();
        }

        public void Send<T>(NetworkPeer peer, T packet) where T: class, IPacket
        {
            byte[] data = m_PacketSerializer.Serialize(packet);

            var packetType = packet.GetPacketType();
            Send(new OutgoingPacket
            {
                Data = data,
                PacketType = packetType,
                Peers = new[] { peer }
            });
        }

        public void Send(OutgoingPacket packet)
        {
            if (packet.Peers.Any(d => !d.EnetPeer.IsSet))
            {
                throw new Exception("Failed to send packet because it contains invalid peers.");
            }

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

        private void HandleIncomingEvent(Event @event)
        {
#if LOG_NETWORK
            m_Logger.LogDebug("Processing network event: " + @event.Type);
#endif
            NetworkPeer networkPeer = null;
            if (@event.Peer.IsSet)
            {
                networkPeer = GetPeerByNetworkId(@event.Peer.ID);
            }

            switch (@event.Type)
            {
                case EventType.Connect:
                    if (!@event.Peer.IsSet)
                    {
#if LOG_NETWORK
                        m_Logger.LogWarning("Peer was not set in Connect event.");
#endif
                    }
                    else
                    {
                        RegisterPeer(new NetworkPeer(@event.Peer));
                    }
                    break;
                case EventType.Timeout:
                case EventType.Disconnect:
                    if (networkPeer != null)
                    {
                        UnregisterPeer(networkPeer);
                    }
                    break;
            }

            NetworkEvent networkEvent = new NetworkEvent(@event, networkPeer);
            m_EventBus.Emit(this, networkEvent);

            if (networkEvent.IsCancelled)
                return;

            m_NetworkEventProcessor.ProcessNetworkEvent(@event, networkPeer);
        }

        private void HandleOutgoingPacket(OutgoingPacket outgoingPacket)
        {
#if LOG_NETWORK
            m_Logger.LogDebug($"[Network] > {outgoingPacket.PacketType.ToString()}");
#endif
            var peers = outgoingPacket.Peers.Select(d => d.EnetPeer).ToArray();

            Packet packet = default;

            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)outgoingPacket.PacketType);
            ms.Write(outgoingPacket.Data.ToArray(), 0, outgoingPacket.Data.Length);
            packet.Create(ms.ToArray(), outgoingPacket.PacketType.GetPacketDescription().PacketFlags);
            ms.Dispose();

            var channelId = (byte)outgoingPacket.PacketType.GetPacketDescription().Channel;

            if (peers.Length == 1)
                peers.First().Send(channelId, ref packet);
            else
                m_Host.Broadcast(channelId, ref packet, peers);
        }

        public void Shutdown(bool waitForQueue = true)
        {
            if (!IsListening)
            {
                return;
            }

            IsListening = false;

            m_Host?.Dispose();
            m_Host = null;
            //todo: implement waitForQueue
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
                return;

            Shutdown(false);
        }

        public IEnumerable<NetworkPeer> GetPeers()
        {
            return m_ConnectedPeers.Where(d => d.IsAuthenticated);
        }

        public IEnumerable<NetworkPeer> GetPendingPeers()
        {
            return m_ConnectedPeers.Where(d => !d.IsAuthenticated);
        }

        public void RegisterPeer(NetworkPeer networkPeer)
        {
            m_ConnectedPeers.Add(networkPeer);
        }

        public void UnregisterPeer(NetworkPeer networkPeer)
        {
            m_ConnectedPeers.Remove(networkPeer);
        }

        public NetworkPeer GetPeerByNetworkId(uint peerID)
        {
            return GetPeers().FirstOrDefault(d => d.EnetPeer.ID == peerID);
        }

        public NetworkPeer GetPeerBySteamId(ulong steamId)
        {
            return GetPeers().FirstOrDefault(d => d.SteamId == steamId);
        }
    }
}