using Microsoft.Extensions.DependencyInjection;

namespace Redcache.Serialization.SystemTextJson
{
    public static class ServiceCollectionRedisExtensions
    {
        public static IRedcacheServiceCollectionBuilder AddRedisJsonSerializer(this IRedcacheServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedcacheSerializer, SystemTextJsonSerializer>();

            return builder;
        }
    }
}