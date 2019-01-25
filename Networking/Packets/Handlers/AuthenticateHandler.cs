using System.Collections.Generic;
using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Map;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets.Serialization;
using ImperialStudio.Core.Networking.Server;
using ImperialStudio.Core.Steam;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticate)]
    public class AuthenticateHandler : BasePacketHandler<AuthenticatePacket>
    {
        private readonly ServerConnectionHandler m_ServerConnectionHandler;
        private readonly IMapManager m_MapManager;
        private readonly ILogger m_Logger;
        private readonly Dictionary<ulong, NetworkPeer> m_PendingAuthentications;

        public AuthenticateHandler(
            IPacketSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            IEventBus eventBus,
            IMapManager mapManager,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ServerConnectionHandler = connectionHandler as ServerConnectionHandler;
            m_MapManager = mapManager;
            m_Logger = logger;
            m_PendingAuthentications = new Dictionary<ulong, NetworkPeer>();

            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
                eventBus.Subscribe<NetworkEvent>(this, HandleNetworkEvent);
                SteamServerComponent.Instance.Server.Auth.OnAuthChange += OnSteamAuthChange;
            }
        }

        private void OnSteamAuthChange(ulong steamId, ulong ownerSteamId, ServerAuth.Status authSessionResponse)
        {
            m_Logger.LogDebug($"Authentication result for \"{steamId}\": {authSessionResponse.ToString()}");

            if (!m_PendingAuthentications.ContainsKey(steamId))
            {
                return;
            }

            var peer = m_PendingAuthentications[steamId];
            m_PendingAuthentications.Remove(steamId);

            if (authSessionResponse != ServerAuth.Status.OK)
            {
                if (authSessionResponse != ServerAuth.Status.AuthTicketCanceled)
                    m_Logger.LogWarning($"Authentication failed for {peer.Name}: {authSessionResponse.ToString()}");

                peer.IsAuthenticated = false;
                m_ServerConnectionHandler.Disconnect(peer, authSessionResponse);
                return;
            }

            peer.SteamId = steamId;
            peer.IsAuthenticated = true;

            m_ServerConnectionHandler.Send(new OutgoingPacket
            {
                PacketType = PacketType.Authenticated,
                Data = new byte[0],
                Peers = new[] { peer }
            });

            var mapPacket = new MapChangePacket { MapName = m_MapManager.CurrentMap };
            m_ServerConnectionHandler.Send(peer, mapPacket);
        }

        private void HandleNetworkEvent(object sender, NetworkEvent @event)
        {
            var enetEvent = @event.EnetEvent;

            if (enetEvent.Type == EventType.Disconnect || enetEvent.Type == EventType.Timeout)
            {
                var networkPeer = m_ServerConnectionHandler.GetPeerByNetworkId(enetEvent.Peer.ID);

                if (networkPeer != null)
                {
                    SteamServerComponent.Instance.Server.Auth.EndSession(networkPeer.SteamId);
                }
            }
        }

        protected override void OnHandleVerifiedPacket(NetworkPeer sender, AuthenticatePacket incomingPacket)
        {
            m_Logger.LogDebug("Received authentication request from user: " + incomingPacket.SteamId);
            m_PendingAuthentications.Add(incomingPacket.SteamId, sender);
        
            if (!SteamServerComponent.Instance.Server.Auth.StartSession(incomingPacket.Ticket, incomingPacket.SteamId))
            {
                m_ServerConnectionHandler.Disconnect(sender, "Could not initialize Steam authentication session.");
            }
        }
    }
}
