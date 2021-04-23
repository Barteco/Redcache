using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Redfish.Internal
{
    internal class RedfishLoggerScope : IRedfishLoggerScope
    {
        private readonly ILogger _logger;
        private readonly IDisposable _scope;
        private readonly Stopwatch _stopwatch;
        private readonly LogLevel _defaultLevel;
        private bool _failed;

        public RedfishLoggerScope(ILogger logger, LogLevel defaultLevel, Dictionary<string, object> state)
        {
            _logger = logger;
            _scope = logger.BeginScope(state);
            _stopwatch = Stopwatch.StartNew();
            _defaultLevel = defaultLevel;
        }

        public void Catch(Exception exception)
        {
            _stopwatch.Stop();
            _logger.LogError(exception, "Redis call failed after {Duration}", _stopwatch.Elapsed);
            _failed = true;
        }

        public void Dispose()
        {
            if (!_failed)
            {
                _stopwatch.Stop();
                _logger.Log(_defaultLevel, "Redis call completed in {Duration}", _stopwatch.Elapsed);
            }
            _scope.Dispose();
        }
    }
}
