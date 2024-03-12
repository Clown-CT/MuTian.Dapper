using Microsoft.AspNetCore.Mvc;
using MuTian.Dapper.ApiTest.Filters;
using MuTian.Dapper.Interfaces;

namespace MuTian.Dapper.ApiTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IDapperClient<TenantDbConnectionHandler> dapperClient)
        {
            _logger = logger;
            DapperClient = dapperClient;
        }

        public IDapperClient<TenantDbConnectionHandler> DapperClient { get; }

        [HttpPost]
        [ServiceFilter(typeof(TenantConnectionResourceFilterAttribute))]
        public IEnumerable<WeatherForecast> Get([FromBody] Tenant tenant)
        {
            var DataTable = DapperClient.ExecuteDataTable("select * from users");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
