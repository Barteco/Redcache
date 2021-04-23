using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Redfish.Decorators;
using Redfish.Internal;
using Redfish.Tests.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Redfish.Tests
{
    public class RedqueueLoggerServiceTests
    {
        private readonly IRedqueue _redqueue;
        private readonly MockLogger _logger;
        private readonly IOptions<LoggingOptions> _options;

        private readonly IRedfishLogger _redfishLogger;
        private readonly RedqueueLoggerService _redqueueDecorator;

        public RedqueueLoggerServiceTests()
        {
            _redqueue = Substitute.For<IRedqueue>();
            _logger = Substitute.For<MockLogger>();
            _options = Substitute.For<IOptions<LoggingOptions>>();
            _options.Value.Returns(new LoggingOptions());

            _redfishLogger = new RedfishLogger(_logger, _options);
            _redqueueDecorator = new RedqueueLoggerService(_redqueue, _redfishLogger);
        }

        [Fact]
        public async Task Log_Publish()
        {
            // Act
            await _redqueueDecorator.Publish("channel", "message");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redqueue.Received().Publish(Arg.Is("channel"), Arg.Is("message")).Wait();
        }

        [Fact]
        public async Task Log_Subscribe()
        {
            // Act
            await _redqueueDecorator.Subscribe<string>("channel", _ => { });

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redqueue.Received().Subscribe(Arg.Is("channel"), Arg.Any<Action<string>>()).Wait();
        }

        [Fact]
        public async Task Log_Unsubscribe()
        {
            // Act
            await _redqueueDecorator.Unsubscribe("channel");

            // Assert
            _logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
            _redqueue.Received().Unsubscribe(Arg.Is("channel")).Wait();
        }
    }
}