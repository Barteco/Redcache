using StackExchange.Redis;
using System.Text.Json;

namespace Redcache.Serialization.SystemTextJson
{
    class SystemTextJsonSerializer : IRedcacheSerializer
    {
        public TValue Deserialize<TValue>(RedisValue cached)
        {
            return JsonSerializer.Deserialize<TValue>(cached);
        }

        public RedisValue Serialize<TValue>(TValue value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}
