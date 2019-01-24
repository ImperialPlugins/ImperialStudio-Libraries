using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using NetStack.Threading;
using System.Linq;
using UnityEngine;
using Event = ENet.Event;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Packets
{
    public sealed class PacketProcessorComponent : MonoBehaviour
    {
        private ConcurrentBuffer m_EventQueue;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            m_EventQueue = new ConcurrentBuffer(8192);
        }

        [AutoInject]
        private ILogger m_Logger;

        [AutoInject]
        private IConnectionHandler m_ConnectionHandler;

        [AutoInject]
        private INetworkEventProcessor m_EventProcessor;

        [AutoInject]
        private IEventBus m_EventBus;

        private bool m_Disposing;

        public volatile bool ShouldFlush;

        private long _frame;

        private void Update()
        {
            _frame++;

            if (m_EventQueue.Count == 0 && m_Disposing)
            {
                Destroy(gameObject);
                return;
            }

            if (m_EventQueue.Count == 0)
            {
                return;
            }

            while (m_EventQueue.Count > 0)
            {
                var @event = (Event)m_EventQueue.Dequeue();
#if LOG_NETWORK
                m_Logger.LogDebug("Processing network event: " + @event.Type);
#endif

                switch (@event.Type)
                {
                    case ENet.EventType.Connect:
                        if (!@event.Peer.IsSet)
                        {
#if LOG_NETWORK
                            m_Logger.LogWarning("Peer was not set in Connect event.");
#endif
                        }
                        else
                        {
                            m_ConnectionHandler.RegisterPeer(new NetworkPeer(@event.Peer));
                        }
                        break;
                    case ENet.EventType.Timeout:
                    case ENet.EventType.Disconnect:
                        var networkPeer = m_ConnectionHandler.GetPeers().FirstOrDefault(d => d.EnetPeer.ID == @event.Peer.ID);
                        if (networkPeer != null)
                        {
                            m_ConnectionHandler.UnregisterPeer(networkPeer);
                        }
                        break;
                }

                NetworkEvent networkEvent = new NetworkEvent(@event);
                m_EventBus.Emit(this, networkEvent);

                if (networkEvent.IsCancelled)
                    continue;

                m_EventProcessor.ProcessEvent(@event);
            }

            m_ConnectionHandler.Flush();
        }

        private void OnDestroy()
        {
            if (!m_Disposing)
                m_ConnectionHandler.Dispose();
        }

        public void EnqueueIncoming(Event @event)
        {
            m_EventQueue.Enqueue(@event);
        }

        public void DestroyAfterQueue()
        {
            m_Disposing = true;
        }
    }
}