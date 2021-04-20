using Microsoft.Extensions.DependencyInjection;

namespace Redfish
{
    public static class ServiceCollectionRedisExtensions
    {
        public static IRedfishServiceCollectionBuilder AddRedisJsonSerializer(this IRedfishServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedfishSerializer, SystemTextJsonSerializer>();

            return builder;
        }
    }
}