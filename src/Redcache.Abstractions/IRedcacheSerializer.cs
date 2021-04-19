using StackExchange.Redis;

namespace Redcache
{
    public interface IRedcacheSerializer
    {
        RedisValue Serialize<TValue>(TValue value);
        TValue Deserialize<TValue>(RedisValue cached);
    }
}