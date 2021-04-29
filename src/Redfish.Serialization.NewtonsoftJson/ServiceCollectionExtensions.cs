using Microsoft.Extensions.DependencyInjection;

namespace Redfish
{
    public static class ServiceCollectionExtensions
    {
        public static IRedfishServiceCollectionBuilder AddNewtonsoftSerializer(this IRedfishServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedfishSerializer, NewtonsoftJsonSerializer>();

            return builder;
        }
    }
}