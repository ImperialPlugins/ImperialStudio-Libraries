using System;
using ENet;
using ImperialStudio.Core.Game;

namespace ImperialStudio.Core.Networking.Packets.Handlers
{
    public abstract class BasePacketHandler: IPacketHandler
    {
        private readonly IGamePlatformAccessor m_GamePlatformAccessor;

        protected BasePacketHandler(IGamePlatformAccessor gamePlatformAccessor)
        {
            m_GamePlatformAccessor = gamePlatformAccessor;
        }

        public void HandlePacket(Peer peer, EPacket packet, byte[] packetData)
        {
            var packetInfo = packet.GetInfo();
            switch (packetInfo.SenderPlatform)
            {
                case GamePlatform.Client | GamePlatform.Server:
                    // do nothing
                    break;
                case GamePlatform.Client when m_GamePlatformAccessor.GamePlatform == GamePlatform.Client:
                    throw new Exception("Server tried to send a client packet");

                case GamePlatform.Server when m_GamePlatformAccessor.GamePlatform == GamePlatform.Server:
                    throw new Exception("Client tried to send a server packet");
            }
        }

        protected abstract void OnHandlePacket(Peer peer, EPacket packet, byte[] packetData);
    }
}