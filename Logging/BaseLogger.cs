using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Reflection;
using UnityEngine;
using ILogger = ImperialStudio.Core.Api.Logging.ILogger;

namespace ImperialStudio.Core.Logging
{
    public abstract class BaseLogger : Api.Logging.ILogger
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
                    return Color.gray;
                case LogLevel.Debug:
                    return Color.grey;
                case LogLevel.Information:
                    return Color.green;
                case LogLevel.Warning:
                    return Color.yellow;
                case LogLevel.Error:
                    return Color.red;
                case LogLevel.Fatal:
                    return Color.red;
            }

            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        public virtual MethodBase GetLoggerCallingMethod()
            => ReflectionExtensions.GetCallingMethod(ignoredLoggingTypes.ToArray(), ignoredLoggingMethods.ToArray());

        public abstract string ServiceName { get; }
    }
}