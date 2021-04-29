using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redfish;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleApplication.HostedServices
{
    public class ClearWeatherService : BackgroundService
    {
        public IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ClearWeatherService> _logger;

        public ClearWeatherService(IServiceScopeFactory serviceScopeFactory,
            ILogger<ClearWeatherService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var redqueue = scope.ServiceProvider.GetRequiredService<IRedqueue>();
                var redcache = scope.ServiceProvider.GetRequiredService<IRedcache>();

                await redqueue.Subscribe<DateTime>("weather_channel", async date =>
                {
                    await redcache.Delete("weather");
                    _logger.LogInformation($"Weather cleared at {date}!");
                });
            }
        }
    }
}