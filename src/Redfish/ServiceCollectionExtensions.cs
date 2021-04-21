using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redfish.Internal;
using Redfish.Services;
using StackExchange.Redis;

namespace Redfish
{
    public static class ServiceCollectionExtensions
    {
        public static IRedfishServiceCollectionBuilder AddRedfish(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = new RedfishServiceCollectionBuilder(services);

            var redisOptions = configuration.Get<RedisOptions>();
            var configurationOptions = RedisOptionsBuilder.BuildConfigurationOptions(redisOptions);
            var connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);

            builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
            builder.Services.AddScoped<IRedcache, RedcacheService>();
            builder.Services.AddScoped<IRedqueue, RedqueueService>();

            return builder;
        }
    }
}