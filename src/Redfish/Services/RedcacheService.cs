using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redfish.Services
{
    internal class RedcacheService : IRedcache
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly IRedfishSerializer _serializer;
        private readonly IDatabase _database;

        public RedcacheService(IConnectionMultiplexer multiplexer, IRedfishSerializer serializer)
        {
            _multiplexer = multiplexer;
            _serializer = serializer;
            _database = _multiplexer.GetDatabase();
        }

        public async Task<bool> Exists(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<Optional<T>> Get<T>(string key, TimeSpan? expiry = null)
        {
            var cachedValue = await _database.StringGetAsync(key);
            if (cachedValue.HasValue)
            {
                return _serializer.Deserialize<T>(cachedValue);
            }
            return Optional<T>.None();
        }

        public async Task<T> Get<T>(string key, Func<T> setter, TimeSpan? expiry = null)
        {
            var cachedValue = await _database.StringGetAsync(key);
            if (cachedValue.HasValue)
            {
                return _serializer.Deserialize<T>(cachedValue);
            }

            var value = setter();
            await Set(key, value, expiry);
            return value;
        }

        public async Task<List<T>> GetList<T>(string key, Range? range = null)
        {
            int start = 0, stop = -1;

            if (range.HasValue)
            {
                start = range.Value.Start.Value;
                stop = range.Value.End.Value;
            }

            var cachedList = await _database.ListRangeAsync(key, start, stop);

            var items = new List<T>();

            foreach (var item in cachedList)
            {
                var value = _serializer.Deserialize<T>(item);
                items.Add(value);
            }

            return items;
        }

        public async Task AppendList<T>(string key, T value)
        {
            var serializedValue = _serializer.Serialize(value);
            await _database.ListRightPushAsync(key, serializedValue);
        }

        public async Task AppendList<T>(string key, T[] values)
        {
            var serializedValues = values.Select(_serializer.Serialize).ToArray();
            await _database.ListRightPushAsync(key, serializedValues);
        }

        public async Task Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serializedValue = _serializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, expiry);
        }

        public async Task Delete(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task DeleteMultiple(string[] keys)
        {
            await _database.KeyDeleteAsync(keys.Select(key => new RedisKey(key)).ToArray());
        }

        public async Task DeleteNamespace(string @namespace)
        {
            foreach (var endpoint in _multiplexer.GetEndPoints())
            {
                var keys = new List<RedisKey>();

                await foreach (var key in _multiplexer.GetServer(endpoint).KeysAsync(pattern: $"{@namespace.TrimEnd(':')}:*"))
                {
                    keys.Add(key);
                }

                await _database.KeyDeleteAsync(keys.ToArray());
            }
        }

        public async Task FlushAll()
        {
            foreach (var endpoint in _multiplexer.GetEndPoints())
            {
                await _multiplexer.GetServer(endpoint).FlushDatabaseAsync();
            }
        }
    }
}
