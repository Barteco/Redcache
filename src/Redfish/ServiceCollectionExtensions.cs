using Microsoft.Extensions.DependencyInjection;
using Redfish.Internal;
using Redfish.Services;
using StackExchange.Redis;

namespace Redfish
{
    public static class ServiceCollectionExtensions
    {
        public static IRedfishServiceCollectionBuilder AddRedis(this IServiceCollection services, RedfishOptions redisOptions)
        {
            var builder = new RedfishServiceCollectionBuilder(services);

            var configurationOptions = RedfishOptionsBuilder.Build(redisOptions);
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));

            return builder;
        }

        public static IRedfishServiceCollectionBuilder AddRedcache(this IRedfishServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedcache, RedcacheService>();

            return builder;
        }

        public static IRedfishServiceCollectionBuilder AddRedqueue(this IRedfishServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedqueue, RedqueueService>();

            return builder;
        }
    }
}