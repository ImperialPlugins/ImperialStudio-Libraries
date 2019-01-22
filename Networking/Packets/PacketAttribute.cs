using System;
using ImperialStudio.Core.Game;

namespace ImperialStudio.Core.Networking.Packets
{
    public class PacketAttribute : Attribute
    {
        public GamePlatform SenderPlatform { get; }
        public int ChannelId { get; }

        public PacketAttribute(GamePlatform senderPlatform, int channelId)
        {
            SenderPlatform = senderPlatform;
            ChannelId = channelId;
        }
    }
}