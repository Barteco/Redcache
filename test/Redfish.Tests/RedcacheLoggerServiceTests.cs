using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Redfish.Decorators;
using Redfish.Internal;
using Redfish.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Redfish.Tests
{
    public class RedcacheLoggerServiceTests
    {
        private readonly IRedcache _redcache;
        private readonly MockLogger _logger;
        private readonly IOptions<LoggingOptions> _options;

        private readonly IRedfishLogger _redfishLogger;
        private readonly RedcacheLoggerService _redcacheDecorator;

        public RedcacheLoggerServiceTests()
        {
            _redcache = Substitute.For<IRedcache>();
            _logger = Substitute.For<MockLogger>();
            _options = Substitute.For<IOptions<LoggingOptions>>();
            _options.Value.Returns(new LoggingOptions());

            _redfishLogger = new RedfishLogger(_logger, _options);
            _redcacheDecorator = new RedcacheLoggerService(_redcache, _redfishLogger);
        }

        [Fact]
        public async Task Log_Exists()
        {
            // Arrange
            _redcache.Exists(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _redcacheDecorator.Exists("key");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().Exists(Arg.Is("key")).Wait();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Log_Get()
        {
            // Arrange
            _redcache.Get<string>(Arg.Any<string>()).Returns("value");

            // Act
            var result = await _redcacheDecorator.Get<string>("key");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().Get<string>(Arg.Is("key")).Wait();
            result.Value.Should().Be("value");
        }

        [Fact]
        public async Task Log_GetSet_WithAbsoluteExpiration()
        {
            // Arrange
            var expiration = DateTime.UtcNow.AddDays(1);
            _redcache.GetOrSet(Arg.Any<string>(), Arg.Any<Func<string>>(), Arg.Any<DateTime>()).Returns("value");

            // Act
            var result = await _redcacheDecorator.GetOrSet("key", () => "value", expiration);

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().GetOrSet(Arg.Is("key"), Arg.Any<Func<string>>(), Arg.Is(expiration)).Wait();
            result.Should().Be("value");
        }

        [Fact]
        public async Task Log_GetSet_WithAbsoluteOffsetExpiration()
        {
            // Arrange
            var expiration = DateTimeOffset.UtcNow.AddDays(1);
            _redcache.GetOrSet(Arg.Any<string>(), Arg.Any<Func<string>>(), Arg.Any<DateTimeOffset>()).Returns("value");

            // Act
            var result = await _redcacheDecorator.GetOrSet("key", () => "value", expiration);

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().GetOrSet(Arg.Is("key"), Arg.Any<Func<string>>(), Arg.Is(expiration)).Wait();
            result.Should().Be("value");
        }

        [Fact]
        public async Task Log_GetSet()
        {
            // Arrange
            _redcache.GetOrSet(Arg.Any<string>(), Arg.Any<Func<string>>(), Arg.Any<TimeSpan>()).Returns("value");

            // Act
            var result = await _redcacheDecorator.GetOrSet("key", () => "value", TimeSpan.FromSeconds(1));

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().GetOrSet(Arg.Is("key"), Arg.Any<Func<string>>(), Arg.Is(TimeSpan.FromSeconds(1))).Wait();
            result.Should().Be("value");
        }

        [Fact]
        public async Task Log_GetList()
        {
            // Arrange
            _redcache.GetList<string>(Arg.Any<string>(), Arg.Any<Range>()).Returns(new List<string> { "value1", "value2" });

            // Act
            var result = await _redcacheDecorator.GetList<string>("key", 3..4);

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().GetList<string>(Arg.Is("key"), Arg.Is(3..4)).Wait();
            result.Should().BeEquivalentTo("value1", "value2");
        }

        [Fact]
        public async Task Log_AppendList_WithSingleItem()
        {
            // Act
            await _redcacheDecorator.AppendList("key", "value");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().AppendList(Arg.Is("key"), Arg.Is("value")).Wait();
        }

        [Fact]
        public async Task Log_AppendList_WithItemsArray()
        {
            // Act
            await _redcacheDecorator.AppendList("key", new[] { "value1", "value2" });

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().AppendList(Arg.Is("key"), Arg.Is<string[]>(x => x.SequenceEqual(new[] { "value1", "value2" }))).Wait();
        }

        [Fact]
        public async Task Log_Set_WithAbsoluteExiration()
        {
            // Arrange
            var expiration = DateTime.UtcNow.AddDays(1);

            // Act
            await _redcacheDecorator.Set("key", "value", expiration);

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().Set(Arg.Is("key"), Arg.Is("value"), Arg.Is(expiration)).Wait();
        }

        [Fact]
        public async Task Log_Set_WithAbsoluteOffsetExiration()
        {
            // Arrange
            var expiration = DateTimeOffset.UtcNow.AddDays(1);

            // Act
            await _redcacheDecorator.Set("key", "value", expiration);

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().Set(Arg.Is("key"), Arg.Is("value"), Arg.Is(expiration)).Wait();
        }

        [Fact]
        public async Task Log_Set()
        {
            // Act
            await _redcacheDecorator.Set("key", "value", TimeSpan.FromSeconds(1));

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().Set(Arg.Is("key"), Arg.Is("value"), Arg.Is(TimeSpan.FromSeconds(1))).Wait();
        }

        [Fact]
        public async Task Log_Delete()
        {
            // Act
            await _redcacheDecorator.Delete("key");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().Delete(Arg.Is("key")).Wait();
        }

        [Fact]
        public async Task Log_DeleteMultiple()
        {
            // Act
            await _redcacheDecorator.DeleteMultiple(new[] { "key1", "key2" });

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().DeleteMultiple(Arg.Is<string[]>(x => x.SequenceEqual(new[] { "key1", "key2" }))).Wait();
        }

        [Fact]
        public async Task Log_DeleteNamespace()
        {
            // Act
            await _redcacheDecorator.DeleteNamespace("ns");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().DeleteNamespace(Arg.Is("ns")).Wait();
        }

        [Fact]
        public async Task Log_FlushAll()
        {
            // Act
            await _redcacheDecorator.FlushAll();

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redcache.Received().FlushAll().Wait();
        }
    }
}