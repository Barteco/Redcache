using Microsoft.Extensions.DependencyInjection;

namespace Redcache.Serialization.Protobuf
{
    public static class ServiceCollectionExtensions
    {
        public static IRedcacheServiceCollectionBuilder AddRedisProtobufSerializer(this IRedcacheServiceCollectionBuilder builder)
        {
            builder.Services.AddScoped<IRedcacheSerializer, ProtobufSerializer>();

            return builder;
        }
    }
}
