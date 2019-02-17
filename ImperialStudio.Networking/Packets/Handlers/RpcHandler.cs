using ImperialStudio.Api.Game;
using ImperialStudio.Api.Logging;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Networking.Components;
using System;
using System.Collections.Generic;

namespace ImperialStudio.Networking.Packets.Handlers
{
    [PacketType(PacketType.Rpc)]
    public class RpcHandler : BasePacketHandler<RpcPacket>
    {
        private readonly IObjectSerializer m_ObjectSerializer;
        private readonly INetworkComponentManager m_NetworkComponentManager;

        public RpcHandler(
            IObjectSerializer objectSerializer,
            IGamePlatformAccessor gamePlatformAccessor,
            IConnectionHandler connectionHandler,
            INetworkComponentManager networkComponentManager,
            ILogger logger) : base(objectSerializer, gamePlatformAccessor, connectionHandler, logger)
        {
            m_ObjectSerializer = objectSerializer;
            m_NetworkComponentManager = networkComponentManager;
        }

        protected override void OnHandleVerifiedPacket(INetworkPeer sender, RpcPacket incomingPacket)
        {
            var componentInstance = m_NetworkComponentManager.GetComponent(incomingPacket.ComponentId);
            var method = componentInstance.GetRcpMethod(incomingPacket.MethodIndex);

            List<object> invokeArgs = new List<object> { sender };
            int index = 0;

            var data = incomingPacket.Arguments;

            foreach (var parameter in method.GetParameters())
            {
                int size = BitConverter.ToInt32(data, index);
                index += sizeof(int);

                var segment = new ArraySegment<byte>(data, index, size);
                var arg = m_ObjectSerializer.Deserialize(segment, parameter.ParameterType);
                invokeArgs.Add(arg);

                index += size;
            }

            method.Invoke(componentInstance, invokeArgs.ToArray());
        }
    }
}