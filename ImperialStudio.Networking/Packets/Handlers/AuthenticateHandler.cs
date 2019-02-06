using System.Collections.Generic;
using ImperialStudio.Networking.Events;
using ImperialStudio.Networking.Server;
#if STEAM_AUTH
using ImperialStudio.Core.Steam;
#endif

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(Packets.PacketType.Authenticate)]
    public class AuthenticateHandler : BasePacketHandler<AuthenticatePacket>
    {
        private readonly ServerConnectionHandler m_ServerConnectionHandler;
        private readonly IMapManager m_MapManager;
        private readonly ILogger m_Logger;
        private readonly Dictionary<ulong, INetworkPeer> m_PendingAuthentications;
        private readonly IEventBus m_EventBus;

        public AuthenticateHandler(
            IObjectSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            IEventBus eventBus,
            IMapManager mapManager,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_EventBus = eventBus;
            m_ServerConnectionHandler = connectionHandler as ServerConnectionHandler;
            m_MapManager = mapManager;
            m_Logger = logger;
            m_PendingAuthentications = new Dictionary<ulong, INetworkPeer>();

            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
            {
#if STEAM_AUTH
                eventBus.Subscribe<NetworkEvent>(this, OnNetworkEvent);
                SteamServerComponent.Instance.Server.Auth.OnAuthChange += OnSteamAuthChange;
#endif
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
                    m_Logger.LogWarning($"Authentication failed for {peer}: {authSessionResponse.ToString()}");

                peer.IsAuthenticated = false;
                m_ServerConnectionHandler.Disconnect(peer, authSessionResponse);
                return;
            }

            peer.SteamId = steamId;
            peer.IsAuthenticated = true;

            m_ServerConnectionHandler.Send(new OutgoingPacket
            {
                PacketId = (byte) Packets.PacketType.Authenticated,
                Data = new byte[0],
                Peers = new[] { peer }
            });

            var mapPacket = new MapChangePacket { MapName = m_MapManager.CurrentMap };
            m_ServerConnectionHandler.Send(peer, mapPacket);

            m_EventBus.Emit(this, new PeerAuthenicatedEvent(peer));
        }

#if STEAM_AUTH
        private void OnNetworkEvent(object sender, NetworkEvent @event)
        {
            var enetEvent = @event.EnetEvent;

            if (enetEvent.Type == EventType.Disconnect || enetEvent.Type == EventType.Timeout)
            {
                var networkPeer = m_ServerConnectionHandler.GetPeerByNetworkId(enetEvent.Peer.ID, true);

                if (networkPeer != null)
                {
                    m_Logger.LogInformation("Ending steam session with: " + networkPeer.SteamId);
                    SteamServerComponent.Instance.Server.Auth.EndSession(networkPeer.SteamId);
                }
            }
        }
#endif

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, AuthenticatePacket incomingPacket)
        {
            m_Logger.LogDebug("Received authentication request from user: " + incomingPacket.SteamId);

#if STEAM_AUTH
            ulong steamId = incomingPacket.SteamId;
#else
            ulong steamId = sender.Id; 
#endif

            if (sender.IsAuthenticated
                || m_PendingAuthentications.ContainsKey(steamId))
            {
                m_ServerConnectionHandler.Disconnect(sender, "Already connected to server!");
                return;
            }

            m_PendingAuthentications.Add(steamId, sender);
            sender.Username = incomingPacket.Username;

#if STEAM_AUTH
            if (!SteamServerComponent.Instance.Server.Auth.StartSession(incomingPacket.Ticket, incomingPacket.SteamId))
            {
                m_ServerConnectionHandler.Disconnect(sender, "Could not initialize Steam authentication session.");
            }
#else
            OnSteamAuthChange(steamId, steamId, ServerAuth.Status.OK);
#endif
        }
    }
}
