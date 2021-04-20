using Microsoft.Extensions.Configuration;
using Redfish.Internal;
using StackExchange.Redis;
using System;

namespace Redfish.Tests.Fixtures
{
    public class RedfishFixture : IDisposable
    {
        public IConnectionMultiplexer Multiplexer { get; }

        public RedfishFixture()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var redisOptions = config.GetSection("Redis").Get<RedfishOptions>();
            var configurationOptions = RedfishOptionsBuilder.Build(redisOptions);
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