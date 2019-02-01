using System;
using Castle.Windsor;
using ImperialStudio.Core.Api.Logging;
using ImperialStudio.Core.Reflection;
using UnityEngine;

namespace ImperialStudio.Core.Logging
{
    public abstract class FormattedLogger : BaseLogger
    {
        public readonly object OutputLock = new object();

        protected FormattedLogger(IWindsorContainer container) : base(container)
        {
            SkipTypeFromLogging(GetType());
        }

        public override void OnLog(string message, LogLevel level = LogLevel.Information, Exception exception = null,
                                   params object[] bindings)
        {
            if (message != null)
                WriteLine(level, message, Color.white, bindings);

            if (exception != null)
                WriteLine(level, exception.ToString(), Color.red);
        }

        public void WriteLine(LogLevel level, string message, Color? color = null, params object[] bindings)
        {
            lock (OutputLock)
            {
                WriteColored("[", Color.white);
                WriteColored(GetLogLevelPrefix(level), GetLogLevelColor(level));
                WriteColored("] ", Color.white);

                WriteColored("[", Color.white);
                WriteColored(GetLoggerCallingMethod().GetDebugName(), Color.gray);
                WriteColored("] ", Color.white);

                WriteLineColored(message, color, bindings);
                Console.ResetColor();
            }
        }

        public void Write(string format, Color? color = null, params object[] bindings)
        {
            WriteColored(format, color, bindings);
        }

        protected abstract void WriteColored(string format, Color? color = null, params object[] bindings);

        protected abstract void WriteLineColored(string format, Color? color = null, params object[] bindings);
    }
}