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
            return await _database.KeyExistsAsync(key).ConfigureAwait(false);
        }

        public async Task<Optional<T>> Get<T>(string key)
        {
            var cachedValue = await _database.StringGetAsync(key).ConfigureAwait(false);
            if (cachedValue.HasValue)
            {
                return _serializer.Deserialize<T>(cachedValue);
            }
            return Optional<T>.None();
        }

        public async Task<T> GetOrSet<T>(string key, Func<T> setter, DateTime absoluteExpiration)
        {
            if (absoluteExpiration < DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteExpiration), "Expiration date must be a future date");
            }

            var slidingExpiration = absoluteExpiration - DateTime.UtcNow;
            return await GetOrSet(key, setter, slidingExpiration).ConfigureAwait(false);
        }

        public async Task<T> GetOrSet<T>(string key, Func<T> setter, DateTimeOffset absoluteExpiration)
        {
            if (absoluteExpiration < DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteExpiration), "Expiration date must be a future date");
            }

            var slidingExpiration = absoluteExpiration - DateTime.UtcNow;
            return await GetOrSet(key, setter, slidingExpiration).ConfigureAwait(false);
        }

        public async Task<T> GetOrSet<T>(string key, Func<T> setter, TimeSpan? slidingExpiration = null)
        {
            if (slidingExpiration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(slidingExpiration), "Expiration must be a positive value");
            }

            var cachedValue = await _database.StringGetAsync(key).ConfigureAwait(false);
            if (cachedValue.HasValue)
            {
                return _serializer.Deserialize<T>(cachedValue);
            }

            var value = setter();
            await Set(key, value, slidingExpiration).ConfigureAwait(false);
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

            var cachedList = await _database.ListRangeAsync(key, start, stop).ConfigureAwait(false);

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
            await _database.ListRightPushAsync(key, serializedValue).ConfigureAwait(false);
        }

        public async Task AppendList<T>(string key, T[] values)
        {
            var serializedValues = values.Select(_serializer.Serialize).ToArray();
            await _database.ListRightPushAsync(key, serializedValues).ConfigureAwait(false);
        }

        public async Task Set<T>(string key, T value, DateTime absoluteExpiration)
        {
            if (absoluteExpiration < DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteExpiration), "Expiration date must be a future date");
            }

            var slidingExpiration = absoluteExpiration - DateTime.UtcNow;
            await Set(key, value, slidingExpiration).ConfigureAwait(false);
        }

        public async Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
        {
            if (absoluteExpiration < DateTime.UtcNow)
            {
                throw new ArgumentOutOfRangeException(nameof(absoluteExpiration), "Expiration date must be a future date");
            }

            var slidingExpiration = absoluteExpiration - DateTime.UtcNow;
            await Set(key, value, slidingExpiration).ConfigureAwait(false);
        }

        public async Task Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
        {
            if (slidingExpiration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(slidingExpiration), "Expiration must be a positive value");
            }

            var serializedValue = _serializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, slidingExpiration).ConfigureAwait(false);
        }

        public async Task Delete(string key)
        {
            await _database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task DeleteMultiple(string[] keys)
        {
            await _database.KeyDeleteAsync(keys.Select(key => new RedisKey(key)).ToArray()).ConfigureAwait(false);
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

                await _database.KeyDeleteAsync(keys.ToArray()).ConfigureAwait(false);
            }
        }

        public async Task FlushAll()
        {
            foreach (var endpoint in _multiplexer.GetEndPoints())
            {
                await _multiplexer.GetServer(endpoint).FlushDatabaseAsync().ConfigureAwait(false);
            }
        }
    }
}