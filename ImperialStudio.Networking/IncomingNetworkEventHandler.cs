using System;
using System.Collections.Generic;
using Castle.Windsor;
using ENet;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Extensions.DependencyInjection;
using ImperialStudio.Networking.Packets;
using ImperialStudio.Networking.Packets.Handlers;

namespace ImperialStudio.Networking
{
    public sealed class IncomingNetworkEventHandler : IIncomingNetworkEventHandler
    {
        private readonly IWindsorContainer m_Container;
        private readonly ILogger m_Logger;
        private readonly Dictionary<byte, ICollection<IPacketHandler>> m_PacketHandlers;
        private bool m_Inited;
        private IConnectionHandler m_ConnectionHandler;

        public IncomingNetworkEventHandler(IWindsorContainer container, ILogger logger)
        {
            m_Container = container;
            m_Logger = logger;
            m_PacketHandlers = new Dictionary<byte, ICollection<IPacketHandler>>();
        }

        public void EnsureLoaded()
        {
            if (m_Inited)
                return;

            m_ConnectionHandler = m_Container.Resolve<IConnectionHandler>();
            RegisterPacketHandler<AuthenticateHandler>();
            RegisterPacketHandler<AuthenticatedHandler>();
            RegisterPacketHandler<PingHandler>();
            RegisterPacketHandler<PongHandler>();
            RegisterPacketHandler<MapChangeHandler>();
            RegisterPacketHandler<TerminateHandler>();
            RegisterPacketHandler<WorldUpdateHandler>();
            RegisterPacketHandler<InputUpdateHandler>();
            RegisterPacketHandler<RpcHandler>();

            m_Inited = true;
        }

        public void RegisterPacketHandler<T>() where T : class, IPacketHandler
        {
            var instance = m_Container.Activate<T>();

            if (!m_PacketHandlers.ContainsKey(instance.PacketId))
            {
                m_PacketHandlers.Add(instance.PacketId, new List<IPacketHandler>());
            }

            m_PacketHandlers[instance.PacketId].Add(instance);
        }

        public void ProcessNetworkEvent(Event @event, INetworkPeer networkPeer)
        {
            if (@event.Type == EventType.Receive)
                HandleReceive(@event, networkPeer);
        }

        private void HandleReceive(Event @event, INetworkPeer networkPeer)
        {
            byte[] incomingPacket = new byte[@event.Packet.Length];
            @event.Packet.CopyTo(incomingPacket);
            @event.Packet.Dispose();


            byte packetType = incomingPacket[0];
            byte[] incomingPacketBody = new byte[incomingPacket.Length - 1];

            Buffer.BlockCopy(incomingPacket, 1, incomingPacketBody, 0, incomingPacketBody.Length);
            HandleIncomingPacket(networkPeer, packetType, incomingPacketBody, @event.ChannelID);
        }

        private void HandleIncomingPacket(INetworkPeer peer, byte packetId, byte[] packetBody, byte channelId)
        {
            var packetDescription = m_ConnectionHandler.GetPacketDescription(packetId);
#if LOG_NETWORK
            m_Logger.LogDebug($"[Network] < {packetDescription.Name}");
#endif
            if (!m_PacketHandlers.ContainsKey(packetId))
            {
#if LOG_NETWORK
                m_Logger.LogWarning($"Failed to handle packet \"{packetId}\" from {peer}: No matching handler was found.");
#endif
                return;
            }

            var handlers = m_PacketHandlers[packetId];
            foreach (var handler in handlers)
            {
                handler.HandlePacket(new IncomingPacket
                {
                    ChannelId = channelId,
                    PacketId = (byte) packetId,
                    Data = packetBody,
                    Peer = peer
                });
            }
        }
    }
}