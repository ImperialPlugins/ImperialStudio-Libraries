using Castle.Windsor;
using ENet;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using System;
using ImperialStudio.Core.Steam;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Networking.Server
{
    public sealed class ServerConnectionHandler : BaseConnectionHandler
    {
        public string Name { get; private set; }
        public byte MaxPlayers { get; set; }

        public ServerConnectionHandler(ILogger logger, IEventBus eventBus, IWindsorContainer container) : base(logger, eventBus, container)
        {
            m_Logger = logger;
        }

        private readonly ILogger m_Logger;
        public const byte MaxPlayersUpperLimit = byte.MaxValue;
        public void Host(ServerListenParameters listenParameters)
        {
            Name = listenParameters.Name;
            MaxPlayers = listenParameters.MaxPlayers;

            if (IsListening)
            {
                throw new Exception("Server is already listening.");
            }

            Address address = new Address { Port = listenParameters.Port };

            if (listenParameters.Host != "0.0.0.0" 
                && !address.SetHost(listenParameters.Host))
            {
                throw new Exception("Failed to resolve host: " + listenParameters.Host);
            }

            var host = GetOrCreateHost();
            host.Create(address, MaxPlayersUpperLimit);

            StartListening();
            m_Logger.LogInformation($"Hosted server: {listenParameters.Host}:{listenParameters.Port}");
        }
    }
}
