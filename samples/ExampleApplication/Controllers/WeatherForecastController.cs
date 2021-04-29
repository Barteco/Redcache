using Microsoft.AspNetCore.Mvc;
using Redfish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IRedcache _redcache;
        private readonly IRedqueue _redqueue;

        public WeatherForecastController(IRedcache redcache, IRedqueue redqueue)
        {
            _redcache = redcache;
            _redqueue = redqueue;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return await _redcache.GetOrSet("weather", () =>
            {
                var rng = new Random();
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    ForecastDate = DateTime.UtcNow,
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            }, TimeSpan.FromMinutes(1));
        }

        [HttpGet("clear")]
        public async Task<OkObjectResult> Clear()
        {
            await _redqueue.Publish("weather_channel", DateTime.UtcNow);
            return Ok("Weather cleared");
        }
    }
}
