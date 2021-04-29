using FluentAssertions;
using FluentAssertions.Extensions;
using Redfish.Services;
using Redfish.Tests.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Redfish.Tests
{
    public class RedqueueServiceTests : IClassFixture<RedfishFixture>
    {
        private readonly RedfishFixture _fixture;
        private readonly SystemTextJsonSerializer _serializer;

        public RedqueueServiceTests(RedfishFixture fixture)
        {
            _fixture = fixture;
            _serializer = new SystemTextJsonSerializer();
        }

        [Fact]
        public async Task Subscribe_WhenMessageIsPublished_ExecutesHandler()
        {
            var mq = new RedqueueService(_fixture.Multiplexer, _serializer);
            var channel = _fixture.GetRandomKey();
            int counter = 0;

            await mq.Subscribe<int>(channel, x => counter += x);
            await mq.Publish(channel, 10);

            Action assertion = () => counter.Should().Be(10);
            assertion.Should().NotThrowAfter(5.Seconds(), 100.Milliseconds());
        }

        [Fact]
        public async Task Subscribe_WhenMultipleMessagesArePublished_ExecutesHandlerMultipleTimes()
        {
            var mq = new RedqueueService(_fixture.Multiplexer, _serializer);
            var channel = _fixture.GetRandomKey();
            int counter = 0;

            await mq.Subscribe<int>(channel, x => counter += x);
            await mq.Publish(channel, 10);
            await mq.Publish(channel, 10);
            await mq.Publish(channel, 10);

            Action assertion = () => counter.Should().Be(30);
            assertion.Should().NotThrowAfter(5.Seconds(), 100.Milliseconds());
        }

        [Fact]
        public async Task Subscribe_WhenMessageIsPublishedBeforeSubscription_DoesNotExecuteHandler()
        {
            var mq = new RedqueueService(_fixture.Multiplexer, _serializer);
            var channel = _fixture.GetRandomKey();
            int counter = 0;

            await mq.Publish(channel, 10);
            await mq.Subscribe<int>(channel, x => counter += x);

            Action assertion = () => counter.Should().Be(0);
            assertion.Should().NotThrowAfter(5.Seconds(), 100.Milliseconds());
        }

        [Fact]
        public async Task Unsubscribe_StopsExecutingHandler()
        {
            var mq = new RedqueueService(_fixture.Multiplexer, _serializer);
            var channel = _fixture.GetRandomKey();
            int counter = 0;

            await mq.Subscribe<int>(channel, x => counter += x);
            await mq.Unsubscribe(channel);
            await mq.Publish(channel, 10);
            await Task.Delay(1.Seconds());

            counter.Should().Be(0);
        }
    }
}
