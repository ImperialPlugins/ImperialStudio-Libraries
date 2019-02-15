using System;
using System.IO;

namespace ImperialStudio.Api.Networking
{
    public interface INetworkComponent
    {
        void Read(Span<byte> newState);

        void Write(Stream stream);
    }
}