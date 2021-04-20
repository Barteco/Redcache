using StackExchange.Redis;

namespace Redfish
{
    public interface IRedfishSerializer
    {
        RedisValue Serialize<TValue>(TValue value);
        TValue Deserialize<TValue>(RedisValue cached);
    }
}