using StackExchange.Redis;
using System.Text.Json;

namespace Redfish.Serialization.SystemTextJson
{
    internal class SystemTextJsonSerializer : IRedfishSerializer
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
