using Redfish.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redfish.Decorators
{
    internal class RedqueueLoggerService : IRedqueue
    {
        private readonly IRedqueue _redqueue;
        private readonly IRedfishLogger _logger;

        public RedqueueLoggerService(IRedqueue redqueue, IRedfishLogger logger)
        {
            _redqueue = redqueue;
            _logger = logger;
        }

        public async Task Publish<T>(string channel, T message)
        {
            using var scope = _logger.BeginScope(new ()
            {
                ["Action"] = "Publish",
                ["Channel"] = channel,
                ["Message"] = message
            });

            try
            {
                await _redqueue.Publish(channel, message).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task Subscribe<T>(string channel, Action<T> handler)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["Action"] = "Subscribe",
                ["Channel"] = channel
            });

            try
            {
                await _redqueue.Subscribe(channel, handler).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task Unsubscribe(string channel)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["Action"] = "Unsubscribe",
                ["Channel"] = channel
            });

            try
            {
                await _redqueue.Unsubscribe(channel).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }
    }
}