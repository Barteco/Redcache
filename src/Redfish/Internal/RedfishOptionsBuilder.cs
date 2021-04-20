using StackExchange.Redis;

namespace Redfish.Internal
{
    internal static class RedfishOptionsBuilder
    {
        public static ConfigurationOptions Build(RedfishOptions redisOptions)
        {
            var configurationOptions = ConfigurationOptions.Parse(redisOptions.ConnectionString);
            configurationOptions.AllowAdmin = true;
            configurationOptions.DefaultDatabase = redisOptions.DefaultDatabase;

            return configurationOptions;
        }
    }
}