using StackExchange.Redis;
using Newtonsoft.Json;

namespace Redfish
{
    internal class NewtonsoftJsonSerializer : IRedfishSerializer
    {
        public TValue Deserialize<TValue>(RedisValue cached)
        {
            return JsonConvert.DeserializeObject<TValue>(cached);
        }

        public RedisValue Serialize<TValue>(TValue value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
