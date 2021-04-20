using Microsoft.Extensions.Configuration;
using Redcache.Internal;
using StackExchange.Redis;
using System;

namespace Redcache.Tests.Fixtures
{
    public class RedisFixture : IDisposable
    {
        public IConnectionMultiplexer Multiplexer { get; }

        public RedisFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var redisOptions = config.GetSection("Redis").Get<RedcacheOptions>();
            var configurationOptions = ConfigurationOptionsBuilder.Build(redisOptions);
            Multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        }

        public string GetRandomNamespace()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetRandomKey(string @namespace = null)
        {
            var key = Guid.NewGuid().ToString();
            return @namespace != null ? $"{@namespace}:{key}" : key;
        }

        public void Dispose()
        {
            Multiplexer?.Dispose();
        }
    }
}