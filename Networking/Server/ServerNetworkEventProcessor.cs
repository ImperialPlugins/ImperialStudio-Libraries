using ENet;
using Facepunch.Steamworks;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Steam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImperialStudio.Core.Networking.Packets;

namespace ImperialStudio.Core.Networking.Server
{
    public sealed class ServerNetworkEventProcessor : INetworkEventProcessor
    {
        private Dictionary<ulong, Peer> m_AuthenticationPending = new Dictionary<ulong, Peer>();
        private Dictionary<ulong, Peer> m_AuthenticatedPeers = new Dictionary<ulong, Peer>();

        private readonly ILogger m_Logger;

        public bool IsAuthenticated(Peer peer)
        {
            return m_AuthenticatedPeers.Any(d => d.Value.ID == peer.ID);
        }

        public ServerNetworkEventProcessor(ILogger logger)
        {
            m_Logger = logger;
            SteamServerComponent.Instance.Server.Auth.OnAuthChange = (steamId, ownerSteamId, authSessionResponse) =>
            {
                m_Logger.LogDebug("Authorization result for \"" + steamId + "\": " + authSessionResponse.ToString());

                if (!m_AuthenticationPending.ContainsKey(steamId))
                {
                    m_Logger.LogWarning("Authorization failed because peer was not found: \"" + steamId + "\"");
                    return;
                }

                var peer = m_AuthenticationPending[steamId];

                if (authSessionResponse != ServerAuth.Status.OK)
                {
                    m_AuthenticationPending.Remove(steamId);
                    m_Logger.LogWarning("Authentication failed for  peer \"" + steamId + "\": " + authSessionResponse.ToString());
                    peer.DisconnectNow(0); //todo: send proper disconnect packet
                }
            };
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
                    break;
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
            ulong steamId = 0;

            foreach (var pair in m_AuthenticationPending)
            {
                if (pair.Value.ID == @event.Peer.ID)
                {
                    steamId = pair.Key;
                }
            }

            if (steamId != 0)
            {
                m_AuthenticationPending.Remove(steamId);
            }

            foreach (var pair in m_AuthenticatedPeers)
            {
                if (pair.Value.ID == @event.Peer.ID)
                {
                    steamId = pair.Key;
                }
            }

            if (steamId != 0)
            {
                m_AuthenticatedPeers.Remove(steamId);
            }

            if (steamId > 0)
            {
                SteamServerComponent.Instance.Server.Auth.EndSession(steamId);
            }
        }

        private void HandleReceive(Event @event)
        {
            if (@event.ChannelID == 1)
            {
                byte[] buffer = new byte[@event.Packet.Length];
                @event.Packet.CopyTo(buffer);
                @event.Packet.Dispose();

                MemoryStream ms = new MemoryStream(buffer);
                EPacket packet = (EPacket)ms.ReadByte();

                byte[] packetData = new byte[Math.Max(buffer.Length - 1, 0)];

                if (packetData.Length > 0)
                    ms.Read(packetData, 0, packetData.Length);

                ms.Dispose();
                HandlePacket(@event.Peer, packet, packetData);
            }
        }

        private void HandlePacket(Peer peer, EPacket packet, byte[] packetData)
        {
            m_Logger.LogDebug("Received packet: " + packet);
            MemoryStream ms = new MemoryStream(packetData);

            switch (packet)
            {
                case EPacket.Authenticate:
                    ulong steamIdToRemove = 0;

                    foreach (var pair in m_AuthenticationPending)
                    {
                        if (pair.Value.ID == peer.ID)
                        {
                            steamIdToRemove = pair.Key;
                            break;
                        }
                    }

                    if (steamIdToRemove > 0)
                    {
                        m_AuthenticationPending.Remove(steamIdToRemove);
                        // Really needed to cancel connection?
                        peer.Reset();
                        break;
                    }

                    byte[] steamIdData = new byte[sizeof(ulong)];
                    ms.Read(steamIdData, 0, steamIdData.Length);

                    ulong steamId = BitConverter.ToUInt64(steamIdData, 0);
                    if (m_AuthenticationPending.ContainsKey(steamId))
                    {
                        peer.Reset();
                        m_AuthenticationPending.Remove(steamId);
                        break;
                    }

                    m_AuthenticationPending.Add(steamId, peer);

                    m_Logger.LogDebug("Received authorization request from user: " + steamId);

                    byte[] ticketData = new byte[packetData.Length - ms.Position];
                    ms.Read(ticketData, 0, ticketData.Length);

                    if (!SteamServerComponent.Instance.Server.Auth.StartSession(ticketData, steamId))
                    {
                        m_Logger.Log("Failed to start authorization session for user: " + steamId);
                        break;
                    }

                    break;
            }

            ms.Dispose();
        }
    }
}