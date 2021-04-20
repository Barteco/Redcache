using StackExchange.Redis;

namespace Redcache.Internal
{
    static class ConfigurationOptionsBuilder
    {
        public static ConfigurationOptions Build(RedcacheOptions redisOptions)
        {
            var configurationOptions = ConfigurationOptions.Parse(redisOptions.ConnectionString);
            configurationOptions.AllowAdmin = true;
            configurationOptions.DefaultDatabase = redisOptions.DefaultDatabase;

            return configurationOptions;
        }
    }
}