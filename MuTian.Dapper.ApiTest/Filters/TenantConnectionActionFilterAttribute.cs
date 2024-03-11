using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlX.XDevAPI.Common;
using System.Text;
using System.Text.Json;

namespace MuTian.Dapper.ApiTest.Filters
{
    public class TenantConnectionActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly TenantDbConnectionHandler _tenantDbConnectionHandler;

        private Dictionary<int, ConnectionSettings> database = new Dictionary<int, ConnectionSettings>() 
        { 
            { 1,new ConnectionSettings { DatabaseType=DatabaseType.SqlServer,ConnectionString= "Data Source=.;Initial Catalog=DapperTestDb;Integrated Security=True;TrustServerCertificate=true" } } 
        };
        public TenantConnectionActionFilterAttribute(TenantDbConnectionHandler tenantDbConnectionHandler)
        {
            _tenantDbConnectionHandler = tenantDbConnectionHandler;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var tenant = context.ActionArguments["tenant"] as Tenant;
            _tenantDbConnectionHandler.SetConnectionSettings(database[tenant.TenantId]);
            await next();
        }
    }
}
