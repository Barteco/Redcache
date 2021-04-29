using FluentAssertions;
using Redfish.Services;
using Redfish.Tests.Fakes;
using Redfish.Tests.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Redfish.Tests
{
    public class SerializationTests : IClassFixture<RedfishFixture>
    {
        private readonly RedfishFixture _fixture;
        private readonly PersonFaker _faker;
        private readonly ProtobufSerializer _protobufSerializer;
        private readonly SystemTextJsonSerializer _systemTextJsonSerializer;
        private readonly NewtonsoftJsonSerializer _newtonsoftJsonSerializer;

        private const string ProtobufSerializerName = "Protobuf";
        private const string SystemTextJsonSerializerName = "SystemTextJson";
        private const string NewtonsoftJsonSerializerName = "NewtonsoftJson";

        public SerializationTests(RedfishFixture fixture)
        {
            _fixture = fixture;
            _faker = new PersonFaker();
            _protobufSerializer = new ProtobufSerializer();
            _systemTextJsonSerializer = new SystemTextJsonSerializer();
            _newtonsoftJsonSerializer = new NewtonsoftJsonSerializer();
        }

        [Theory]
        [InlineData(ProtobufSerializerName)]
        [InlineData(SystemTextJsonSerializerName)]
        [InlineData(NewtonsoftJsonSerializerName)]
        public async Task Serilize_SimpleObject(string serilizerName)
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, SerializerFactory(serilizerName));
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value");
            var cached = await redcache.Get<string>(key);

            // Assert
            cached.Value.Should().Be("value");
        }

        [Theory]
        [InlineData(ProtobufSerializerName)]
        [InlineData(SystemTextJsonSerializerName)]
        [InlineData(NewtonsoftJsonSerializerName)]
        public async Task Serializes_ComplexObject(string serilizerName)
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, SerializerFactory(serilizerName));
            var key = _fixture.GetRandomKey();
            var person = _faker.GenerateOne();

            // Act
            await redcache.Set(key, person);
            var cached = await redcache.Get<Person>(key);

            // Assert
            cached.Value.Should().BeEquivalentTo(person);
        }

        private IRedfishSerializer SerializerFactory(string serilizerName)
        {
            switch (serilizerName)
            {
                case ProtobufSerializerName:
                    return _protobufSerializer;
                case SystemTextJsonSerializerName:
                    return _systemTextJsonSerializer;
                case NewtonsoftJsonSerializerName:
                    return _newtonsoftJsonSerializer;
                default:
                    throw new ArgumentException("Unrecognized serializer");
            }
        }
    }
}