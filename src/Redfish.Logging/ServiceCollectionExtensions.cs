using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redfish.Decorators;
using Redfish.Internal;

namespace Redfish
{
    public static class ServiceCollectionExtensions
    {
        public static IRedfishServiceCollectionBuilder AddLogging(this IRedfishServiceCollectionBuilder builder, IConfiguration options)
        {
            builder.Services.Configure<LoggingOptions>(options);
            builder.Services.AddScoped<IRedfishLogger, RedfishLogger>();
            builder.Services.Decorate<IRedcache, RedcacheLoggerService>();
            builder.Services.Decorate<IRedqueue, RedqueueLoggerService>();

            return builder;
        }
    }
}
