using Microsoft.Extensions.Logging;

namespace Redfish
{
    internal class LoggingOptions
    {
        public LogLevel DefaultLevel { get; set; } = LogLevel.Information;
    }
}
