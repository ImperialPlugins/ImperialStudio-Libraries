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
            byte[] buffer = new byte[@event.Packet.Length];
            @event.Packet.CopyTo(buffer);
            @event.Packet.Dispose();

            MemoryStream ms = new MemoryStream(buffer);
            PacketType packetType = (PacketType)ms.ReadByte();

            byte[] packetData = new byte[Math.Max(buffer.Length - 1, 0)];

            if (packetData.Length > 0)
                ms.Read(packetData, 0, packetData.Length);

            ms.Dispose();
            HandleIncomingPacket(networkPeer, packetType, packetData, @event.ChannelID);
        }

        private void HandleIncomingPacket(NetworkPeer peer, PacketType packetType, byte[] packetData, byte channelId)
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
                    Data = packetData,
                    Peer = peer
                });
            }
        }
    }
}