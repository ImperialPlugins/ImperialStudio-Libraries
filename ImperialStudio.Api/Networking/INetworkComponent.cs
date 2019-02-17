using System;
using System.IO;
using System.Reflection;

namespace ImperialStudio.Api.Networking
{
    public interface INetworkComponent
    {
        void Read(Span<byte> newState);

        void Write(Stream stream);

        MethodInfo GetRcpMethod(byte index);

        MethodInfo GetRcpMethod(string name);
    }
}