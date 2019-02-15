using ImperialStudio.Api.Networking;
using ImperialStudio.Api.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImperialStudio.Networking.State
{
    public class NetworkComponent : INetworkComponent
    {
        private readonly IObjectSerializer m_Serializer;
        private static readonly Dictionary<Type, IEnumerable<SyncVariable>> s_CachedStates = new Dictionary<Type, IEnumerable<SyncVariable>>();

        public NetworkComponent(IObjectSerializer serializer)
        {
            m_Serializer = serializer;
            if (s_CachedStates.ContainsKey(GetType()))
            {
                return;
            }

            var variables = new List<SyncVariable>();

            foreach (var member in GetType()
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!member.GetCustomAttributes(typeof(NetworkStateAttribute), true).Any())
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

            s_CachedStates.Add(GetType(), variables);
        }

        public void Read(Span<byte> newState)
        {
            var states = s_CachedStates[GetType()];
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
            var states = s_CachedStates[GetType()];

            //Parallel.ForEach(states, state =>
            foreach (var state in states)
            {
                var value = state.GetValue(this);
                byte[] serializedVariable = m_Serializer.Serialize(value);
                byte[] lengthBytes = BitConverter.GetBytes(serializedVariable.Length);

                stream.Write(lengthBytes, 0, lengthBytes.Length);
                stream.Write(serializedVariable, 0, serializedVariable.Length);
            }
            //);
        }
    }
}