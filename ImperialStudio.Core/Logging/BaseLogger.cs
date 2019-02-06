using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using ImperialStudio.Api.Logging;
using ImperialStudio.Core.Reflection;

namespace ImperialStudio.Core.Logging
{
    public abstract class BaseLogger : ILogger
    {
        private static readonly ICollection<Type> ignoredLoggingTypes = new HashSet<Type>
        {
            typeof(BaseLogger),
            typeof(LoggerProxy),
            typeof(FormattedLogger),
            typeof(LoggingExtensions)
        };

        private static readonly ICollection<MethodBase> ignoredLoggingMethods = new HashSet<MethodBase>
        {
        };

        protected BaseLogger(IWindsorContainer container)
        {
            Container = container;
            SkipTypeFromLogging(GetType());
        }

        public IWindsorContainer Container { get; }

        public void Log(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                        params object[] arguments)
        {
            if (!IsEnabled(level))
                return;

            OnLog(message, level, exception, arguments);
        }

        public virtual bool IsEnabled(LogLevel level)
        {
            if (logLevels.ContainsKey(level))
                return logLevels[level];

            return true;
        }

        private Dictionary<LogLevel, bool> logLevels = new Dictionary<LogLevel, bool>();
        public virtual void SetEnabled(LogLevel level, bool enabled)
        {
            if (logLevels.ContainsKey(level))
                logLevels[level] = enabled;
            else
                logLevels.Add(level, enabled);
        }

        public static void SkipTypeFromLogging(Type type)
        {
            ignoredLoggingTypes.Add(type);
        }

        public static void SkipMethodFromLogging(MethodBase method)
        {
            ignoredLoggingMethods.Add(method);
        }

        public abstract void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                                   params object[] bindings);

        public static string GetLogLevelPrefix(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "Trace";
                case LogLevel.Debug:
                    return "Debug";
                case LogLevel.Information:
                    return "Info";
                case LogLevel.Warning:
                    return "Warn";
                case LogLevel.Error:
                    return "Error";
                case LogLevel.Fatal:
                    return "Fatal";
            }

            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public static Color GetLogLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return Color.LightGray;
                case LogLevel.Debug:
                    return Color.Gray;
                case LogLevel.Information:
                    return Color.Green;
                case LogLevel.Warning:
                    return Color.Yellow;
                case LogLevel.Error:
                    return Color.Red;
                case LogLevel.Fatal:
                    return Color.DarkRed;
            }

            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public virtual MethodBase GetLoggerCallingMethod()
            => ReflectionExtensions.GetCallingMethod(ignoredLoggingTypes.ToArray(), ignoredLoggingMethods.ToArray());

        public abstract string ServiceName { get; }
    }
}