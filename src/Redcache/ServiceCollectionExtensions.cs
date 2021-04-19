using Microsoft.Extensions.DependencyInjection;
using Redcache.Internal;
using StackExchange.Redis;

namespace Redcache
{
    public static class ServiceCollectionExtensions
    {
        public static IRedcacheServiceCollectionBuilder AddRedis(this IServiceCollection services, RedisOptions redisOptions)
        {
            var builder = new RedcacheServiceCollectionBuilder(services);

            var configurationOptions = ConfigurationOptionsBuilder.Build(redisOptions);
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));

            return builder;
        }

        public static IRedcacheServiceCollectionBuilder AddRedcache(this IRedcacheServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedcache, RedcacheService>();

            return builder;
        }
    }
}