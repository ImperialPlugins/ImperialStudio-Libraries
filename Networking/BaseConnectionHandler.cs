using Castle.Windsor;
using ENet;
using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Events;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using ImperialStudio.Core.Networking.Packets;
using ImperialStudio.Core.Steam;
using UnityEngine;
using EventType = ENet.EventType;
using ILogger = ImperialStudio.Core.Logging.ILogger;
using Object = UnityEngine.Object;

namespace ImperialStudio.Core.Networking
{
    public class BaseConnectionHandler : IConnectionHandler
    {
        private readonly Queue<OutgoingPacket> m_OutgoingQueue;

        public bool IsListening { get; private set; }
        protected Host m_Host { get; private set; }

        private Thread m_NetworkThread;
        private PacketProcessorComponent m_PacketProcessor;
        private volatile bool m_Flush;

        private readonly ILogger m_Logger;
        private readonly IWindsorContainer m_Container;

        public BaseConnectionHandler(ILogger logger, IEventBus eventBus, IWindsorContainer container)
        {
            eventBus.Subscribe<ApplicationQuitEvent>(this, (s, e) => { Dispose(); });

            m_Logger = logger;
            m_Container = container;
            m_OutgoingQueue = new Queue<OutgoingPacket>();
        }

        public void EnqueueOutgoing(OutgoingPacket packet)
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
                m_Host.Service(15, out var netEvent);

                if (netEvent.Type != EventType.None)
                {
                    m_Logger.LogInformation("Network event: " + netEvent.Type);
                    m_PacketProcessor.EnqueueIncoming(netEvent);
                }

                while (m_OutgoingQueue.Count > 0)
                {
                    var outgoingPacket = m_OutgoingQueue.Dequeue();
                    foreach (var peer in outgoingPacket.Peers)
                    {
                        var packet = outgoingPacket.Packet;
                        peer.Send(outgoingPacket.ChannelId, ref packet);

                    }
                }

                if (m_Flush)
                {
                    m_Host.Flush();
                    m_Flush = false;
                }
            }
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

            if (waitForQueue)
            {
                m_PacketProcessor.DestroyAfterQueue();
            }
            else
            {
                Object.Destroy(m_PacketProcessor.gameObject);
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

        public virtual void Dispose()
        {
            if (!IsListening)
                return;

            Shutdown(false);
        }
    }
}