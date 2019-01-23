using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using NetStack.Threading;
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
                var @event = (Event) m_EventQueue.Dequeue();
                m_Logger.LogInformation("Processing network event: " + @event.Type);

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