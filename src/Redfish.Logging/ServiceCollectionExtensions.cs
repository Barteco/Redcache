using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redfish.Decorators;

namespace Redfish
{
    public static class ServiceCollectionExtensions
    {
        public static IRedfishServiceCollectionBuilder AddRedcacheLogging(this IRedfishServiceCollectionBuilder builder, IConfiguration options)
        {
            builder.Services.Configure<LoggingOptions>(options);
            builder.Services.Decorate<IRedcache, RedcacheLoggerService>();
            builder.Services.Decorate<IRedqueue, RedqueueLoggerService>();

            return builder;
        }
    }
}
