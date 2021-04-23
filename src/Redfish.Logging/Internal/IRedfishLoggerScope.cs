using System;

namespace Redfish.Internal
{
    internal interface IRedfishLoggerScope : IDisposable
    {
        void Catch(Exception exception);
    }
}