using System;
using System.Diagnostics;
using ImperialStudio.Core.Logging;

namespace ImperialStudio.Core.Util
{
    public class SpeedBenchmark : IDisposable
    {
        public string Name { get; }
        private readonly ILogger m_Logger;

        private readonly Stopwatch _stopwatch;

        public SpeedBenchmark(string name, ILogger logger)
        {
            m_Logger = logger;

            Name = name;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            m_Logger.LogDebug($"[Benchmark] {Name}: {_stopwatch.Elapsed.TotalMilliseconds}ms");
        }
    }
}