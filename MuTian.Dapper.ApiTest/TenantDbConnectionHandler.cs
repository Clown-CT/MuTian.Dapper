using MuTian.Dapper.Interfaces;

namespace MuTian.Dapper.ApiTest
{
    public class TenantDbConnectionHandler : IDbConnectionHandler
    {
        private ConnectionSettings ConnectionSettings { set; get; }
        public ConnectionSettings GetConnectionSettings()
        {
            return ConnectionSettings;
        }

        public void SetConnectionSettings(ConnectionSettings connectionSettings)
        {
            this.ConnectionSettings = connectionSettings;
        }
    }
}
