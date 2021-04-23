using System;
using System.Collections.Generic;

namespace Redfish.Internal
{
    internal interface IRedfishLogger
    {
        IRedfishLoggerScope BeginScope(Dictionary<string, object> state);
        void Log(string message, params object[] args);
        void LogError(Exception exception, string message, params object[] args);
    }
}