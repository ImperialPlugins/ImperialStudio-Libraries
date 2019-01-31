using Castle.Windsor;
using ENet;
using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Packets;
using ImperialStudio.Core.Networking.Packets.Handlers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImperialStudio.Core.Networking
{
    public sealed class NetworkEventHandler : INetworkEventHandler
    {
        private readonly IWindsorContainer m_Container;
        private readonly ILogger m_Logger;
        private readonly Dictionary<PacketType, ICollection<IPacketHandler>> m_PacketHandlers;
        private bool m_Inited;
        public NetworkEventHandler(IWindsorContainer container, ILogger logger)
        {
            m_Container = container;
            m_Logger = logger;
            m_PacketHandlers = new Dictionary<PacketType, ICollection<IPacketHandler>>();
        }

        public void EnsureLoaded()
        {
            if (m_Inited)
                return;

            RegisterPacketHandler<AuthenticateHandler>();
            RegisterPacketHandler<AuthenticatedHandler>();
            RegisterPacketHandler<PingHandler>();
            RegisterPacketHandler<PongHandler>();
            RegisterPacketHandler<MapChangeHandler>();
            RegisterPacketHandler<TerminateHandler>();
            RegisterPacketHandler<WorldUpdateHandler>();

            m_Inited = true;
        }

        public void RegisterPacketHandler<T>() where T : class, IPacketHandler
        {
            var instance = m_Container.Activate<T>();

            if (!m_PacketHandlers.ContainsKey(instance.PacketType))
            {
                m_PacketHandlers.Add(instance.PacketType, new List<IPacketHandler>());
            }

            m_PacketHandlers[instance.PacketType].Add(instance);
        }

        public void ProcessNetworkEvent(Event @event, NetworkPeer networkPeer)
        {
            if (@event.Type == EventType.Receive)
                HandleReceive(@event, networkPeer);
        }

        private void HandleReceive(Event @event, NetworkPeer networkPeer)
        {
            byte[] incomingPacket = new byte[@event.Packet.Length];
            @event.Packet.CopyTo(incomingPacket);
            @event.Packet.Dispose();


            PacketType packetType = (PacketType)incomingPacket[0];
            byte[] incomingPacketBody = new byte[incomingPacket.Length - 1];

            Buffer.BlockCopy(incomingPacket, 1, incomingPacketBody, 0, incomingPacketBody.Length);
            HandleIncomingPacket(networkPeer, packetType, incomingPacketBody, @event.ChannelID);
        }

        private void HandleIncomingPacket(NetworkPeer peer, PacketType packetType, byte[] packetBody, byte channelId)
        {
#if LOG_NETWORK
            m_Logger.LogDebug($"[Network] < {packetType.ToString()}");
#endif
            if (!m_PacketHandlers.ContainsKey(packetType))
            {
#if LOG_NETWORK
                m_Logger.LogWarning($"Failed to handle packet \"{packetType}\" from {peer.Name}: No matching handler was found.");
#endif
                return;
            }

            var handlers = m_PacketHandlers[packetType];
            foreach (var handler in handlers)
            {
                handler.HandlePacket(new IncomingPacket
                {
                    Channel = (NetworkChannel)channelId,
                    PacketType = packetType,
                    Data = packetBody,
                    Peer = peer
                });
            }
        }
    }
}