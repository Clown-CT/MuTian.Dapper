using MuTian.Dapper.Interfaces;

namespace MuTian.Dapper.Test
{
    public class MainDbConnectionHandler : IDbConnectionHandler
    {
        public ConnectionSettings GetConnectionSettings()
        {
            return ConnectionFactory.GetConnectionSettings("DB:MainDbConfig");
        }
    }
}
