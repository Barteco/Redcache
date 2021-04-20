using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;

namespace Redfish.Internal
{
    internal class RedfishServiceCollectionBuilder : IRedfishServiceCollectionBuilder
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        public RedfishServiceCollectionBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}