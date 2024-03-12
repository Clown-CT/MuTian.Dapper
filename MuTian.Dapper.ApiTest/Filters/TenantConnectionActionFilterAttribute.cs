using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlX.XDevAPI.Common;
using System.Text;
using System.Text.Json;

namespace MuTian.Dapper.ApiTest.Filters
{
    public class TenantConnectionResourceFilterAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly TenantDbConnectionHandler _tenantDbConnectionHandler;

        private Dictionary<int, ConnectionSettings> database = new Dictionary<int, ConnectionSettings>() 
        { 
            { 1,new ConnectionSettings { DatabaseType=DatabaseType.SqlServer,ConnectionString= "Data Source=.;Initial Catalog=DapperTestDb;Integrated Security=True;TrustServerCertificate=true" } } 
        };
        public TenantConnectionResourceFilterAttribute(TenantDbConnectionHandler tenantDbConnectionHandler)
        {
            _tenantDbConnectionHandler = tenantDbConnectionHandler;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            context.HttpContext.Request.EnableBuffering();
            context.HttpContext.Request.Body.Position=0;
            var stream = context.HttpContext.Request.Body;
            using StreamReader reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            context.HttpContext.Request.Body.Position = 0;
            var tenant = JsonSerializer.Deserialize<Tenant>(json,new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
            _tenantDbConnectionHandler.SetConnectionSettings(database[tenant.TenantId]);
            await next();
        }
    }
}
