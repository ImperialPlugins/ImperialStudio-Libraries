using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Game;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Networking.Events;
using ImperialStudio.Core.Networking.Packets.Serialization;
using ImperialStudio.Core.Steam;
using System.Collections.Generic;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    [PacketType(PacketType.Authenticate)]
    public class AuthenticateHandler : BasePacketHandler<AuthenticatePacket>
    {
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly ILogger m_Logger;
        private readonly Dictionary<ulong, Peer> m_AuthenticationPending = new Dictionary<ulong, Peer>();

        public AuthenticateHandler(
            IPacketSerializer packetSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            IEventBus eventBus,
            ILogger logger) : base(packetSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ConnectionHandler = connectionHandler;
            m_Logger = logger;
            eventBus.Subscribe<NetworkEvent>(this, HandleNetworkEvent);

            if (gamePlatformAccessor.GamePlatform == GamePlatform.Server)
                SteamServerComponent.Instance.Server.Auth.OnAuthChange += OnSteamAuthChange;
        }

        private void OnSteamAuthChange(ulong steamId, ulong ownerSteamId, ServerAuth.Status authSessionResponse)
        {
            {
                m_Logger.LogDebug("Authentication result for \"" + steamId + "\": " + authSessionResponse.ToString());

                if (!m_AuthenticationPending.ContainsKey(steamId))
                {
                    m_Logger.LogWarning("Authentication failed because peer was not found: \"" + steamId + "\"");
                    return;
                }

                var peer = m_AuthenticationPending[steamId];

                if (authSessionResponse != ServerAuth.Status.OK)
                {
                    m_AuthenticationPending.Remove(steamId);
                    m_Logger.LogWarning("Authentication failed for  peer \"" + steamId + "\": " + authSessionResponse.ToString());
                    peer.DisconnectNow(0); //todo: send proper disconnect packet
                    return;
                }

                m_ConnectionHandler.Send(new OutgoingPacket
                {
                    PacketType = PacketType.Authenticated,
                    Data = new byte[0],
                    Peers = new[] { peer }
                });
            };
        }

        private void HandleNetworkEvent(object sender, NetworkEvent @event)
        {
            var enetEvent = @event.EnetEvent;

            ulong steamId = 0;

            foreach (var pair in m_AuthenticationPending)
            {
                if (pair.Value.ID == enetEvent.Peer.ID)
                {
                    steamId = pair.Key;
                }
            }

            if (steamId != 0)
            {
                m_AuthenticationPending.Remove(steamId);
                SteamServerComponent.Instance.Server.Auth.EndSession(steamId);
            }
        }

        protected override void OnHandleVerifiedPacket(Peer sender, AuthenticatePacket incomingPacket)
        {
            ulong steamIdToRemove = 0;

            foreach (var pair in m_AuthenticationPending)
            {
                if (pair.Value.ID == sender.ID)
                {
                    steamIdToRemove = pair.Key;
                    break;
                }
            }

            if (steamIdToRemove > 0)
            {
                m_AuthenticationPending.Remove(steamIdToRemove);
                // Really needed to cancel connection?
                sender.Reset();
                return;
            }

            if (m_AuthenticationPending.ContainsKey(incomingPacket.SteamId))
            {
                sender.Reset();
                m_AuthenticationPending.Remove(incomingPacket.SteamId);
                return;
            }

            m_AuthenticationPending.Add(incomingPacket.SteamId, sender);
            m_Logger.LogDebug("Received authentication request from user: " + incomingPacket.SteamId);

            if (!SteamServerComponent.Instance.Server.Auth.StartSession(incomingPacket.Ticket, incomingPacket.SteamId))
            {
                //todo: send auth failed
                m_Logger.Log("Failed to start authentication session for user: " + incomingPacket.SteamId);
                return;
            }
        }
    }
}
