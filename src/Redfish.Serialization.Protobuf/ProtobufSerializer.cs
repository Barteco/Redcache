using ProtoBuf;
using StackExchange.Redis;
using System.IO;

namespace Redfish.Serialization.Protobuf
{
    internal class ProtobufSerializer : IRedfishSerializer
    {
        public TValue Deserialize<TValue>(RedisValue cached)
        {
            return Serializer.Deserialize<TValue>(cached);
        }

        public RedisValue Serialize<TValue>(TValue value)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }
    }
}