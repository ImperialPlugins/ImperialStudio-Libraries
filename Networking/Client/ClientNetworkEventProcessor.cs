using ENet;
using ImperialStudio.Core.Steam;
using System;
using System.IO;
using ImperialStudio.Core.Networking.Packets;

namespace ImperialStudio.Core.Networking.Client
{
    public sealed class ClientNetworkEventProcessor : INetworkEventProcessor
    {
        private readonly ClientConnectionHandler m_ConnectionHandler;

        public ClientNetworkEventProcessor(IConnectionHandler connectionHandler)
        {
            m_ConnectionHandler = (ClientConnectionHandler)connectionHandler;
        }

        public void ProcessEvent(Event @event)
        {
            switch (@event.Type)
            {
                case EventType.None:
                    break;
                case EventType.Connect:
                    SendSteamAuthenticationTicket();
                    break;
                case EventType.Timeout:
                case EventType.Disconnect:
                    m_ConnectionHandler.Dispose();
                    break;
                case EventType.Receive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendSteamAuthenticationTicket()
        {
            var clientId = SteamClientComponent.Instance.Client.SteamId;
            var steamIdBytes = BitConverter.GetBytes(clientId);

            var ticket = SteamClientComponent.Instance.Client.Auth.GetAuthSessionTicket();
            m_ConnectionHandler.SetSessionAuthTicket(ticket);

            MemoryStream ms = new MemoryStream();
            ms.Write(steamIdBytes, 0, steamIdBytes.Length);
            ms.Write(ticket.Data, 0, ticket.Data.Length);

            SendPacket(EPacket.Authenticate, ms.ToArray());
            ms.Dispose();
        }

        private void SendPacket(EPacket type, byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)type);
            ms.Write(data, 0, data.Length);

            Packet packet = default;
            packet.Create(ms.ToArray());
            ms.Dispose();

            m_ConnectionHandler.EnqueueOutgoing(new OutgoingPacket
            {
                ChannelId = 1,
                Packet = packet,
                Peers = new[] { m_ConnectionHandler.ServerPeer }
            });

            packet.Dispose();
        }
    }
}