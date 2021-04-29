using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Redfish.Internal
{
    internal class RedfishLogger : IRedfishLogger
    {
        private readonly ILogger<RedfishLogger> _logger;
        private readonly LoggingOptions _options;

        public RedfishLogger(ILogger<RedfishLogger> logger, IOptions<LoggingOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public void Log(string message, params object[] args)
        {
            _logger.Log(_options.DefaultLevel, message, args);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        public IRedfishLoggerScope BeginScope(Dictionary<string, object> state)
        {
            return new RedfishLoggerScope(_logger, _options.DefaultLevel, state);
        }
    }
}
