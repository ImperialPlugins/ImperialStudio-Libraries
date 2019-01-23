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
        private readonly Dictionary<PacketType, IPacketHandler> m_PacketHandlers;

        public NetworkEventProcessor(IWindsorContainer container, ILogger logger, IConnectionHandler connectionHandler)
        {
            m_Container = container;
            m_Logger = logger;
            m_ConnectionHandler = connectionHandler;
            m_PacketHandlers = new Dictionary<PacketType, IPacketHandler>();

            RegisterPacketHandlers();
        }

        private void RegisterPacketHandlers()
        {
            RegisterPacketHandler<AuthenticateHandler>();
            RegisterPacketHandler<AuthenticatedHandler>();
        }

        public void RegisterPacketHandler<T>() where T : class, IPacketHandler
        {
            var instance = m_Container.Activate<T>();

            if (m_PacketHandlers.ContainsKey(instance.PacketType))
            {
                throw new Exception($"A packet handler for \"{instance.PacketType}\" exists already!");
            }

            m_PacketHandlers.Add(instance.PacketType, instance);
        }

        public void ProcessEvent(Event @event)
        {
            switch (@event.Type)
            {
                case EventType.None:
                    break;
                case EventType.Connect:
                    break;
                case EventType.Timeout:
                case EventType.Disconnect:
                    HandleDisconnect(@event);
                    break;
                case EventType.Receive:
                    HandleReceive(@event);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleDisconnect(Event @event)
        {
            if (m_ConnectionHandler.IsAuthenticated(@event.Peer))
            {
                m_ConnectionHandler.Unauthenticate(@event.Peer);
            }
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
            HandlePacket(@event.Peer, packetType, packetData, @event.ChannelID);
        }

        private void HandlePacket(Peer peer, PacketType packetType, byte[] packetData, byte channelId)
        {
            m_Logger.LogDebug("Received packet: " + packetType);
            if (!m_PacketHandlers.ContainsKey(packetType))
            {
                m_Logger.LogWarning($"Failed to handle packet \"{packetType}\" from peer #{peer.ID}: No matching handler was found.");
                return;
            }

            var handler = m_PacketHandlers[packetType];
            handler.HandlePacket(new IncomingPacket
            {
                ChannelId = channelId,
                PacketType = packetType,
                Data = packetData,
                Peer = peer
            });
        }
    }
}