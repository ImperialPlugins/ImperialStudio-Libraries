using System;
using System.Linq;
using Castle.Windsor;
using ImperialStudio.Api.Logging;
using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Extensions.DependencyInjection;

namespace ImperialStudio.Core.Logging
{
    public class LoggerProxy : ServiceProxy<ILogger>, ILogger
    {
        public LoggerProxy(IWindsorContainer container) : base(container)
        {
        }

        public void Log(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                        params object[] arguments)
        {
            foreach (ILogger service in ProxiedServices)
            {
                if (service != this)
                    service.Log(message, level, exception, arguments);
            }
        }

        public bool IsEnabled(LogLevel level)
        {
            return ProxiedServices.Any(c => c.IsEnabled(level));
        }

        public void SetEnabled(LogLevel level, bool enabled)
        {
            throw new NotSupportedException("Not supported on proxy provider.");
        }

        public string ServiceName => "LoggerProxy";
    }
}