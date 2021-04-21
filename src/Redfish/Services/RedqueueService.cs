using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Redfish.Services
{
    internal class RedqueueService : IRedqueue
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly IRedfishSerializer _serializer;
        private readonly IDatabase _database;
        private readonly ISubscriber _subscriber;

        public RedqueueService(IConnectionMultiplexer multiplexer, IRedfishSerializer serializer)
        {
            _multiplexer = multiplexer;
            _serializer = serializer;
            _database = _multiplexer.GetDatabase();
            _subscriber = _multiplexer.GetSubscriber();
        }

        public async Task Publish<T>(string channel, T message)
        {
            var value = _serializer.Serialize(message);
            await _database.PublishAsync(channel, value).ConfigureAwait(false);
        }

        public async Task Subscribe<T>(string channel, Action<T> handler)
        {
            await _subscriber.SubscribeAsync(channel, (_, value) =>
            {
                var message = _serializer.Deserialize<T>(value);
                handler(message);
            }).ConfigureAwait(false);
        }

        public async Task Unsubscribe(string channel)
        {
            await _subscriber.UnsubscribeAsync(channel).ConfigureAwait(false);
        }
    }
}
