using ENet;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Client;
using ImperialStudio.Core.Networking.Packets.Serialization;
using System;
using System.Linq;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    public abstract class BasePacketHandler<T> : BasePacketHandler where T : IPacket
    {
        private readonly IConnectionHandler m_ConnectionHandler;

        protected sealed override void OnHandleVerifiedPacket(IncomingPacket incomingPacket)
        {
            T deserialized;
            try
            {
                deserialized = PacketSerializer.Deserialize<T>(incomingPacket.Data);
            }
            catch (Exception ex)
            {
                Reject(incomingPacket, "Failed to deserialize: " + ex.Message);
                return;
            }

            OnHandleVerifiedPacket(incomingPacket.Peer, deserialized);
        }

        protected BasePacketHandler(
            IPacketSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger)
            : base(packetSerializer,
                gamePlatformAccessor,
                connectionHandler,
                logger)
        {
            m_ConnectionHandler = connectionHandler;
        }

        protected abstract void OnHandleVerifiedPacket(NetworkPeer sender, T incomingPacket);
    }

    public abstract class BasePacketHandler : IPacketHandler
    {
        protected IPacketSerializer PacketSerializer { get; }
        public PacketType PacketType { get; }

        private readonly IGamePlatformAccessor m_GamePlatformAccessor;
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly ILogger m_Logger;

        protected BasePacketHandler(IPacketSerializer packetSerializer, IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            ILogger logger)
        {
            PacketSerializer = packetSerializer;
            PacketType = ((PacketTypeAttribute[])GetType().GetCustomAttributes(typeof(PacketTypeAttribute), false)).First().PacketType;
            m_GamePlatformAccessor = gamePlatformAccessor;
            m_ConnectionHandler = connectionHandler;
            m_Logger = logger;
        }

        public void HandlePacket(IncomingPacket incomingPacket)
        {
            OnVerifyPacket(incomingPacket);

            if (incomingPacket.IsVerified)
                OnHandleVerifiedPacket(incomingPacket);
        }

        protected virtual void OnVerifyPacket(IncomingPacket incomingPacket)
        {
            var currentPlatform = m_GamePlatformAccessor.GamePlatform;
            var packetDescription = incomingPacket.PacketType.GetPacketDescription();

            // Validate channel
            if (incomingPacket.Channel != packetDescription.Channel)
            {
                Reject(incomingPacket, $"Channel mismatch: received: {(byte)incomingPacket.Channel}, expected: {(byte)packetDescription.Channel}");
                return;
            }

            // Validate authentication
            if (packetDescription.NeedsAuthentication)
            {
                if (!incomingPacket.Peer.IsAuthenticated)
                {
                    Reject(incomingPacket, "Unauthenticated peer");
                    return;
                }
            }

            // Validate packet direction
            switch (packetDescription.Direction)
            {
                case PacketDirection.ClientToServer when currentPlatform == GamePlatform.Client:
                    {
                        Reject(incomingPacket, "Server tried to send a client packet");
                        return;
                    }

                case PacketDirection.ServerToClient when currentPlatform == GamePlatform.Server:
                    {
                        Reject(incomingPacket, "Client tried to send a server packet");
                        return;
                    }

                case PacketDirection.ClientToClient:
                    if (currentPlatform == GamePlatform.Server)
                    {
                        Reject(incomingPacket, "Client tried to send a client packet");
                        return;
                    }

                    var clientConnectionHandler = (ClientConnectionHandler)m_ConnectionHandler;
                    if (incomingPacket.Peer.EnetPeer.ID == clientConnectionHandler.ServerPeer.EnetPeer.ID)
                    {
                        Reject(incomingPacket, "Server tried to send a client packet");
                        return;
                    }

                    break;

                case PacketDirection.Any:
                    break;
            }

            incomingPacket.IsVerified = true;
        }

        protected void Reject(IncomingPacket incomingPacket, string reason)
        {
            m_Logger.LogWarning($"Dropped packet \"{incomingPacket.PacketType.ToString()}\" from {incomingPacket.Peer.Name}: {reason}");
        }

        protected abstract void OnHandleVerifiedPacket(IncomingPacket incomingPacket);
    }
}