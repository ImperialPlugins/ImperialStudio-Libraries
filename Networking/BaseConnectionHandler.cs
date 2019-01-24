using Castle.Windsor;
using ENet;
using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Events;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Packets;
using ImperialStudio.Core.Networking.Packets.Handlers;
using ImperialStudio.Core.Networking.Packets.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using EventType = ENet.EventType;
using ILogger = ImperialStudio.Core.Logging.ILogger;
using Object = UnityEngine.Object;

namespace ImperialStudio.Core.Networking
{
    public abstract class BaseConnectionHandler : IConnectionHandler
    {
        private readonly Queue<OutgoingPacket> m_OutgoingQueue;
        private readonly ICollection<NetworkPeer> m_ConnectedPeers;

        public bool IsListening { get; private set; }
        protected Host m_Host { get; private set; }

        private Thread m_NetworkThread;
        private PacketProcessorComponent m_PacketProcessor;
        private volatile bool m_Flush;

        private readonly IPacketSerializer m_PacketSerializer;
        private readonly ILogger m_Logger;
        private readonly IWindsorContainer m_Container;

        protected BaseConnectionHandler(IPacketSerializer packetSerializer, ILogger logger, IEventBus eventBus, IWindsorContainer container)
        {
            eventBus.Subscribe<ApplicationQuitEvent>(this, (s, e) => { Dispose(); });

            m_PacketSerializer = packetSerializer;
            m_Logger = logger;
            m_Container = container;
            m_OutgoingQueue = new Queue<OutgoingPacket>();
            m_ConnectedPeers = new List<NetworkPeer>();
        }

        public void Send(NetworkPeer peer, IPacket packet)
        {
            byte[] packetData = m_PacketSerializer.Serialize(packet);

            var packetType = packet.GetPacketType();
            Send(new OutgoingPacket
            {
                Data = packetData,
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
                m_Host.Service(60, out var netEvent);

                if (netEvent.Type != EventType.None)
                {
#if LOG_NETWORK
                    m_Logger.LogDebug("Network event: " + netEvent.Type);
#endif
                    m_PacketProcessor.EnqueueIncoming(netEvent);
                }

                while (m_OutgoingQueue.Count > 0)
                {
                    var outgoingPacket = m_OutgoingQueue.Dequeue();
                    foreach (var networkPeer in outgoingPacket.Peers)
                    {
#if LOG_NETWORK
                        m_Logger.LogDebug($"[Network] > {outgoingPacket.PacketType.ToString()}");
#endif
                        Packet packet = default;

                        MemoryStream ms = new MemoryStream();
                        ms.WriteByte((byte)outgoingPacket.PacketType);
                        ms.Write(outgoingPacket.Data, 0, outgoingPacket.Data.Length);

                        packet.Create(ms.ToArray());
                        ms.Dispose();

                        var channelId = (byte)outgoingPacket.PacketType.GetPacketDescription().Channel;
                        networkPeer.EnetPeer.Send(channelId, ref packet);
                    }
                }

                if (m_Flush)
                {
                    m_Host.Flush();
                    m_Flush = false;
                }

                Thread.Sleep(20);
            }
        }

        public void Shutdown(bool waitForQueue = true)
        {
            if (!IsListening)
            {
                return;
            }

            IsListening = false;

            foreach (var peer in m_ConnectedPeers)
            {
                peer.EnetPeer.DisconnectNow(0);
            }

            m_Host?.Dispose();
            m_Host = null;

            if (m_PacketProcessor != null)
            {
                if (waitForQueue)
                {
                    m_PacketProcessor.DestroyAfterQueue();
                }
                else
                {
                    Object.Destroy(m_PacketProcessor.gameObject);
                }
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

            var platform = m_Container.Resolve<IGamePlatformAccessor>().GamePlatform;

            GameObject processorObject = new GameObject("PacketProcessor-" + platform);
            m_PacketProcessor = processorObject.AddComponentWithInjection<PacketProcessorComponent>(m_Container);

            m_NetworkThread = new Thread(NetworkUpdate);
            m_NetworkThread.Start();
        }

        protected void StopListening()
        {
            IsListening = false;
            GameObject.Destroy(m_PacketProcessor.gameObject);
        }

        public virtual void Dispose()
        {
            if (!IsListening)
                return;

            Shutdown(false);
        }

        public IEnumerable<NetworkPeer> GetPeers()
        {
            return m_ConnectedPeers;
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
            return m_ConnectedPeers.FirstOrDefault(d => d.EnetPeer.ID == peerID);
        }

        public NetworkPeer GetPeerBySteamId(ulong steamId)
        {
            return m_ConnectedPeers.FirstOrDefault(d => d.SteamId == steamId);
        }
    }
}