using System;
using Castle.Windsor;
using ImperialStudio.Api.Logging;
using ImperialStudio.Core.Logging;
using ImperialStudio.Core.Reflection;
using UnityEngine;

namespace ImperialStudio.Core.UnityEngine.Logging
{
    public class UnityLogger : BaseLogger
    {
        public UnityLogger(IWindsorContainer container) : base(container)
        {
        }

        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null, params object[] bindings)
        {
            LogType logType;

            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                    logType = LogType.Log;
                    break;
                case LogLevel.Warning:
                    logType = LogType.Warning;
                    break;
                case LogLevel.Fatal:
                case LogLevel.Error:
                    logType = exception == null ? LogType.Error : LogType.Exception;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }


            string callingMethod = GetLoggerCallingMethod().GetDebugName();

            string formattedLine = $"[{DateTime.Now}] [{GetLogLevelPrefix(level)}] "
                                   + $"[{callingMethod}] "
                                   + $"{message}";

            Debug.LogFormat(logType, LogOption.NoStacktrace, null, formattedLine, bindings);

            if (exception != null)
            {
                Debug.LogException(exception);
            }
        }

        public override string ServiceName => "UnityLogger";
    }
}