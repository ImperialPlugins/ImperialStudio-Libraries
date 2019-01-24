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
    public sealed class NetworkEventProcessor : INetworkEventProcessor
    {
        private readonly IWindsorContainer m_Container;
        private readonly ILogger m_Logger;
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly Dictionary<PacketType, ICollection<IPacketHandler>> m_PacketHandlers;

        public NetworkEventProcessor(IWindsorContainer container, ILogger logger, IConnectionHandler connectionHandler)
        {
            m_Container = container;
            m_Logger = logger;
            m_ConnectionHandler = connectionHandler;
            m_PacketHandlers = new Dictionary<PacketType, ICollection<IPacketHandler>>();

            RegisterPacketHandlers();
        }

        private void RegisterPacketHandlers()
        {
            RegisterPacketHandler<AuthenticateHandler>();
            RegisterPacketHandler<AuthenticatedHandler>();
            RegisterPacketHandler<PingHandler>();
            RegisterPacketHandler<PongHandler>();
            RegisterPacketHandler<TerminateHandler>();
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

        public void ProcessEvent(Event @event)
        {
            if (@event.Type == EventType.Receive)
                HandleReceive(@event);
        }

        private void HandleReceive(Event @event)
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

            NetworkPeer networkPeer = m_ConnectionHandler.GetPeerByNetworkId(@event.Peer.ID);
            if (networkPeer == null)
            {
                throw new Exception($"Failed to handle packet, peer was not found (peer #{@event.Peer.ID}; IsSet: {@event.Peer.IsSet}).");
            }

            HandlePacket(networkPeer, packetType, packetData, @event.ChannelID);
        }

        private void HandlePacket(NetworkPeer peer, PacketType packetType, byte[] packetData, byte channelId)
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