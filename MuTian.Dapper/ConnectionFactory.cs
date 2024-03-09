using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuTian.Dapper
{
    public class ConnectionFactory
    {
        private readonly static IConfiguration _appSettings;
        private readonly static ConcurrentDictionary<string, ConnectionSettings> _cacheConnectionSettings = new ConcurrentDictionary<string, ConnectionSettings>();

        static ConnectionFactory()
        {
            if (_appSettings == null)
                _appSettings = GetConfiguration("appsettings.json");
        }
        private static IConfiguration GetConfiguration(string settingFileName)
        {
            string settingFileNamePath = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName)).FullName;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingFileNamePath, optional: false, reloadOnChange: true);
            IConfigurationRoot configurationRoot = builder.Build();
            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                if (_cacheConnectionSettings.Count > 0)
                {
                    _cacheConnectionSettings.Clear();
                }
            });
            return configurationRoot;
        }

        public static ConnectionSettings GetConnectionSettings(string settingStr)
        {
            ConnectionSettings? connectionSettings;
            if (!_cacheConnectionSettings.TryGetValue(settingStr, out connectionSettings))
            {
                connectionSettings = _appSettings.GetSection(settingStr).Get<ConnectionSettings>()?? throw new ArgumentNullException("ConnectionSettings is null");
                _cacheConnectionSettings.TryAdd(settingStr, connectionSettings);
                return connectionSettings;
            }
            return connectionSettings;
        }

        public static IDbConnection CreateConnection(ConnectionSettings connectionSettings)
        {
            IDbConnection connection;
            switch (connectionSettings.DatabaseType)
            {
                case DatabaseType.SQLite:
                    connection = new SqliteConnection(connectionSettings.ConnectionString);
                    break;
                case DatabaseType.MySql:
                    connection = new MySqlConnection(connectionSettings.ConnectionString);
                    break;
                case DatabaseType.Oracle:
                    connection = new OracleConnection(connectionSettings.ConnectionString);
                    break;
                case DatabaseType.SqlServer:
                    connection = new SqlConnection(connectionSettings.ConnectionString);
                    break;
                default: throw new ArgumentException("暂时不支持该数据库");
            }
            return connection;
        }
    }
}
