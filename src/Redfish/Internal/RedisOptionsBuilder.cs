﻿using StackExchange.Redis;

namespace Redfish.Internal
{
    internal static class RedisOptionsBuilder
    {
        public static ConfigurationOptions BuildConfigurationOptions(RedisOptions redisOptions)
        {
            var configurationOptions = ConfigurationOptions.Parse(redisOptions.ConnectionString);
            configurationOptions.AllowAdmin = true;
            configurationOptions.DefaultDatabase = redisOptions.DefaultDatabase;

            return configurationOptions;
        }
    }
}