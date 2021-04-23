using FluentAssertions;
using Redfish.Services;
using Redfish.Tests.Fakes;
using Redfish.Tests.Fixtures;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Redfish.Tests
{
    public class RedcacheServiceTests : IClassFixture<RedfishFixture>
    {
        private readonly RedfishFixture _fixture;
        private readonly PersonFaker _faker;
        private readonly SystemTextJsonSerializer _jsonSerializer;
        private readonly ProtobufSerializer _protobufSerializer;

        public RedcacheServiceTests(RedfishFixture fixture)
        {
            _fixture = fixture;
            _faker = new PersonFaker();
            _jsonSerializer = new SystemTextJsonSerializer();
            _protobufSerializer = new ProtobufSerializer();
        }

        [Fact]
        public async Task Exists_WhenAccessingNonexistentKey_ReturnsFalse()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            var keyExists = await redcache.Exists(key);

            // Assert
            keyExists.Should().BeFalse();
        }

        [Fact]
        public async Task Exists_WhenAccessingNonexistentKey_ReturnsTrue()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value");
            var keyExists = await redcache.Exists(key);

            // Assert
            keyExists.Should().BeTrue();
        }

        [Fact]
        public async Task Exists_WhenAccessingKeyBeforeExpiry_ReturnsItsTrue()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value", TimeSpan.FromSeconds(1));
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            var keyExists = await redcache.Exists(key);

            // Assert
            keyExists.Should().BeTrue();
        }

        [Fact]
        public async Task Exists_WhenAccessingKeyAfterExpiry_ReturnsFalse()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value", TimeSpan.FromMilliseconds(100));
            await Task.Delay(TimeSpan.FromMilliseconds(101));
            var keyExists = await redcache.Exists(key);

            // Assert
            keyExists.Should().BeFalse();
        }

        [Fact]
        public async Task Get_WhenAccessingNonexistentKey_ReturnsNull()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            var cached = await redcache.Get<string>(key);

            // Assert
            cached.IsNone.Should().BeTrue();
        }

        [Fact]
        public async Task Get_WhenAccessingNonexistentValueTypeKey_ReturnsNull()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            var cached = await redcache.Get<int>(key);

            // Assert
            cached.IsNone.Should().BeTrue();
        }

        [Fact]
        public async Task Get_WhenAccessingExistentKey_ReturnsItsValue()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value");
            var cached = await redcache.Get<string>(key);

            // Assert
            cached.Value.Should().Be("value");
        }

        [Fact]
        public async Task GetOrSet_WhenAccessingNonexistentKeyWithSetter_ReturnsSetterValue()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            var cachedValue = await redcache.GetOrSet(key, () => "value");

            // Assert
            cachedValue.Should().Be("value");
        }

        [Fact]
        public async Task GetOrSet_WhenUsedMultipleTimesWithDifferentSetters_ReturnsFirstValue()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            var firstCachedValue = await redcache.GetOrSet(key, () => "value1");
            var secondCachedValue = await redcache.GetOrSet(key, () => "value2");

            // Assert
            firstCachedValue.Should().Be("value1");
            secondCachedValue.Should().Be("value1");
        }

        [Fact]
        public async Task Get_WhenAccessingKeyBeforeExpiry_ReturnsItsValue()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value", TimeSpan.FromSeconds(1));
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            var cached = await redcache.Get<string>(key);

            // Assert
            cached.Value.Should().Be("value");
        }

        [Fact]
        public async Task Get_WhenAccessingKeyAfterExpiry_ReturnsNull()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value", TimeSpan.FromMilliseconds(100));
            await Task.Delay(TimeSpan.FromMilliseconds(101));
            var cached = await redcache.Get<string>(key);

            // Assert
            cached.Value.Should().BeNull();
        }

        [Fact]
        public async Task Append_AddsGivenValueToEndOfList()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.AppendList(key, "value1");
            var cachedList1 = await redcache.GetList<string>(key);
            await redcache.AppendList(key, "value2");
            var cachedList2 = await redcache.GetList<string>(key);

            // Assert
            cachedList1.Should().Equal("value1");
            cachedList2.Should().Equal("value1", "value2");
        }

        [Fact]
        public async Task Append_WhenMultipleItems_AddsAllGivenValueToEndOfList()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.AppendList(key, new[] { "value1", "value2" });
            var cachedList1 = await redcache.GetList<string>(key);
            await redcache.AppendList(key, new[] { "value3", "value4" });
            var cachedList2 = await redcache.GetList<string>(key);

            // Assert
            cachedList1.Should().Equal("value1", "value2");
            cachedList2.Should().Equal("value1", "value2", "value3", "value4");
        }

        [Theory]
        [InlineData(0, 4, new[] { 1, 2, 3, 4, 5 })]
        [InlineData(5, 9, new[] { 6, 7, 8, 9, 10 })]
        [InlineData(96, 105, new[] { 97, 98, 99, 100 })]
        [InlineData(110, 120, new int[] { })]
        public async Task GetList_WhenPagerIsSpecified_ReturnsListPage(int from, int to, int[] expectedList)
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.AppendList(key, Enumerable.Range(1, 100).ToArray());
            var cachedList = await redcache.GetList<int>(key, from..to);

            // Assert
            cachedList.Should().Equal(expectedList);
        }

        [Fact]
        public async Task Delete_RemovesKey()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value");
            await redcache.Delete(key);
            var keyExists = await redcache.Exists(key);

            // Assert
            keyExists.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteMultiple_RemovesAllGivenKeys()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key1 = _fixture.GetRandomKey();
            var key2 = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key1, "value");
            await redcache.Set(key2, "value");
            await redcache.DeleteMultiple(new[] { key1, key2 });
            var key1Exists = await redcache.Exists(key1);
            var key2Exists = await redcache.Exists(key2);

            // Assert
            key1Exists.Should().BeFalse();
            key2Exists.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteNamespace_RemovesAllKeysInGivenNamespace()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var ns = _fixture.GetRandomNamespace();
            var key1 = _fixture.GetRandomKey(ns);
            var key2 = _fixture.GetRandomKey(ns);

            // Act
            await redcache.Set(key1, "value");
            await redcache.Set(key2, "value");
            await redcache.DeleteNamespace(ns);
            var key1Exists = await redcache.Exists(key1);
            var key2Exists = await redcache.Exists(key2);

            // Assert
            key1Exists.Should().BeFalse();
            key2Exists.Should().BeFalse();
        }

        [Fact]
        public async Task Set_WhenUsingJsonSerilizer_StoresComplexObject()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _jsonSerializer);
            var key = _fixture.GetRandomKey();
            var person = _faker.GenerateOne();

            // Act
            await redcache.Set(key, person);
            var cached = await redcache.Get<Person>(key);

            // Assert
            cached.Value.Should().BeEquivalentTo(person);
        }

        [Fact]
        public async Task Set_WhenUsingProtobufSerilizer_StoresSimpleObject()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _protobufSerializer);
            var key = _fixture.GetRandomKey();

            // Act
            await redcache.Set(key, "value");
            var cached = await redcache.Get<string>(key);

            // Assert
            cached.Value.Should().Be("value");
        }

        [Fact]
        public async Task Set_WhenUsingProtobufSerilizer_StoresComplexObject()
        {
            // Arrange
            var redcache = new RedcacheService(_fixture.Multiplexer, _protobufSerializer);
            var key = _fixture.GetRandomKey();
            var person = _faker.GenerateOne();

            // Act
            await redcache.Set(key, person);
            var cached = await redcache.Get<Person>(key);

            // Assert
            cached.Value.Should().BeEquivalentTo(person);
        }
    }
}