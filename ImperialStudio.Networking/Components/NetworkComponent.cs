using ENet;
using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using ImperialStudio.Networking.Packets;
using ImperialStudio.Networking.Packets.Handlers;
using ImperialStudio.Networking.State;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using ImperialStudio.Networking.Rpc;

namespace ImperialStudio.Networking.Components
{
    public abstract class NetworkComponent : INetworkComponent
    {
        private int? m_ComponentId;
        public int? ComponentId
        {
            get { return m_ComponentId; }
            set
            {
                if (m_ComponentId != null)
                {
                    m_NetworkComponentManager.UnregisterComponent(m_ComponentId.Value);
                }

                if (value != null)
                {
                    m_NetworkComponentManager.RegisterComponent(value.Value, this);
                }

                m_ComponentId = value;
            }
        }

        private readonly IObjectSerializer m_Serializer;
        private readonly IConnectionHandler m_ConnectionHandler;
        private readonly INetworkComponentManager m_NetworkComponentManager;

        private static readonly Dictionary<Type, IReadOnlyCollection<SyncVariable>> s_CachedVariables = new Dictionary<Type, IReadOnlyCollection<SyncVariable>>();
        private static readonly Dictionary<Type, Dictionary<string, byte>> s_RpcMethodsMappings = new Dictionary<Type, Dictionary<string, byte>>();
        private static readonly Dictionary<Type, Dictionary<byte, MethodInfo>> s_RpcMethods = new Dictionary<Type, Dictionary<byte, MethodInfo>>();
        private static readonly Dictionary<Type, Dictionary<byte, RpcAttribute>> s_RpcOptions = new Dictionary<Type, Dictionary<byte, RpcAttribute>>();

        public NetworkComponent(IObjectSerializer serializer, IConnectionHandler connection, INetworkComponentManager networkComponentManager)
        {
            m_Serializer = serializer;
            m_ConnectionHandler = connection;
            m_NetworkComponentManager = networkComponentManager;

            BuildType();
        }

        private void BuildType()
        {
            BuildVariables();
            BuildMethods();
        }

        private void BuildMethods()
        {
            if (s_RpcMethodsMappings.ContainsKey(GetType()))
                return;

            var mappings = new Dictionary<string, byte>();
            var rpcMethods = new Dictionary<byte, MethodInfo>();
            var rpcAttributes = new Dictionary<byte, RpcAttribute>();

            byte methodIndex = 0;
            foreach (var method in GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                RpcAttribute attribute;
                if ((attribute = (RpcAttribute)method.GetCustomAttributes(typeof(RpcAttribute), true).FirstOrDefault()) == null)
                {
                    continue;
                }

                var normalizedName = method.Name.ToLowerInvariant();

                if (mappings.ContainsKey(normalizedName))
                {
                    throw new Exception($"Duplicate RPC definition: {method} in type: {GetType()} already exists!");
                }

                mappings.Add(normalizedName, methodIndex);
                rpcMethods.Add(methodIndex, method);
                rpcAttributes.Add(methodIndex, attribute);
            }

            s_RpcMethodsMappings.Add(GetType(), mappings);
            s_RpcMethods.Add(GetType(), rpcMethods);
            s_RpcOptions.Add(GetType(), rpcAttributes);
        }

        private void BuildVariables()
        {
            if (s_CachedVariables.ContainsKey(GetType()))
                return;

            var variables = new List<SyncVariable>();

            foreach (var member in GetType()
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!member.GetCustomAttributes(typeof(NetworkVariableAttribute), true).Any())
                {
                    continue;
                }

                switch (member)
                {
                    case PropertyInfo property:
                        variables.Add(new SyncVariable(property));
                        break;

                    case FieldInfo field:
                        variables.Add(new SyncVariable(field));
                        break;
                }
            }

            s_CachedVariables.Add(GetType(), variables);
        }

        public void Read(Span<byte> newState)
        {
            var states = s_CachedVariables[GetType()];
            int index = 0;

            foreach (var syncedVariable in states)
            {
                var lengthBytes = newState.Slice(index, 4);
                var length = BitConverter.ToInt32(lengthBytes.ToArray(), 0);

                index += 4;
                byte[] data = newState.Slice(index, length).ToArray();
                index += length;

                var newValue = m_Serializer.Deserialize(data, syncedVariable.Type);
                syncedVariable.SetValue(this, newValue);
            }
        }

        public void Write(Stream stream)
        {
            var states = s_CachedVariables[GetType()];

            foreach (var state in states)
            {
                var value = state.GetValue(this);
                byte[] serializedVariable = m_Serializer.Serialize(value);
                byte[] lengthBytes = BitConverter.GetBytes(serializedVariable.Length);

                stream.Write(lengthBytes, 0, lengthBytes.Length);
                stream.Write(serializedVariable, 0, serializedVariable.Length);
            }
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public void InvokeRpc(string name, INetworkPeer receiver, PacketFlags? flags, params object[] args)
        {
            GuardComponentId();

            var normalizedName = name.ToLowerInvariant();

            var serializedArgs = SerializeArgs(args);

            var methodIndex = s_RpcMethodsMappings[GetType()][normalizedName];
            var packet = new RpcPacket
            {
                Arguments = serializedArgs,
                ComponentId = ComponentId.Value,
                MethodIndex = methodIndex
            };

            var serializedPacket = m_Serializer.Serialize(packet);

            var packetDescription = m_ConnectionHandler.GetPacketDescription(methodIndex);

            if (flags != null)
            {
                packetDescription = packetDescription.Clone();
                packetDescription.PacketFlags = flags.Value;
            }

            m_ConnectionHandler.Send(new OutgoingPacket
            {
                Data = serializedPacket,
                PacketDescription = packetDescription,
                PacketId = (byte)PacketType.Rpc,
                Peers = new[] { receiver }
            });
        }

        public void InvokeRpc(string name, INetworkPeer receiver, params object[] args)
        {
            InvokeRpc(name, receiver, null, args);
        }

        public void InvokeRpc(string name, uint receiverId, params object[] args)
        {
            var receiver = m_ConnectionHandler.GetPeerByNetworkId(receiverId);
            InvokeRpc(name, receiver, args);
        }

        public void InvokeRpc(string name, uint receiverId, PacketFlags? flags, params object[] args)
        {
            var receiver = m_ConnectionHandler.GetPeerByNetworkId(receiverId);
            InvokeRpc(name, receiver, flags, args);
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public void InvokeRpc(string name, IEnumerable<INetworkPeer> receivers, PacketFlags? flags, params object[] args)
        {
            GuardComponentId();

            var normalizedName = name.ToLowerInvariant();

            var serializedArgs = SerializeArgs(args);

            var methodIndex = s_RpcMethodsMappings[GetType()][normalizedName];
            var packet = new RpcPacket
            {
                Arguments = serializedArgs,
                ComponentId = ComponentId.Value,
                MethodIndex = methodIndex
            };

            var serializedPacket = m_Serializer.Serialize(packet);

            var packetDescription = m_ConnectionHandler.GetPacketDescription(methodIndex);

            if (flags != null)
            {
                packetDescription = packetDescription.Clone();
                packetDescription.PacketFlags = flags.Value;
            }

            m_ConnectionHandler.Send(new OutgoingPacket
            {
                Data = serializedPacket,
                PacketDescription = packetDescription,
                PacketId = (byte)PacketType.Rpc,
                Peers = receivers.ToList()
            });
        }

        public void InvokeRpc(string name, IEnumerable<uint> receivers, PacketFlags flags, params object[] args)
        {
            List<INetworkPeer> networkPeers = new List<INetworkPeer>();
            foreach (var id in receivers)
                networkPeers.Add(m_ConnectionHandler.GetPeerByNetworkId(id));

            InvokeRpc(name, networkPeers, flags, args);
        }

        public void InvokeRpc(string name, PacketFlags flags, params object[] args)
        {
            InvokeRpc(name, m_ConnectionHandler.GetPeers(true), flags, args);
        }

        public MethodInfo GetRcpMethod(byte index)
        {
            return s_RpcMethods[GetType()][index];
        }

        public MethodInfo GetRcpMethod(string name)
        {
            var normalizedName = name.ToLowerInvariant();
            var methodIndex = s_RpcMethodsMappings[GetType()][normalizedName];

            return s_RpcMethods[GetType()][methodIndex];
        }

        private byte[] SerializeArgs(object[] args)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (var arg in args)
                {
                    byte[] serializedArg = m_Serializer.Serialize(arg);
                    var sizeBytes = BitConverter.GetBytes(serializedArg.Length);

                    ms.Write(sizeBytes, 0, sizeBytes.Length);
                    ms.Write(serializedArg, 0, serializedArg.Length);
                }

                return ms.ToArray();
            }
        }

        private void GuardComponentId()
        {
            if (ComponentId == null)
            {
                throw new Exception("ComponentId was not set up yet.");
            }
        }
    }
}