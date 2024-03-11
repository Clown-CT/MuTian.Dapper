using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace MuTian.Dapper.Interfaces
{
    public interface IDapperSqlServerClient : IDapperClient
    {
        void ExecuteSqlBulkCopy(string tableName, DataTable table, int batchSize = 1000, int? bulkCopyTimeout = null);
        void ExecuteSqlBulkCopy(string tableName, DataTable table, int batchSize = 1000, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, bool isBeginTransaction = true, int? bulkCopyTimeout = null);
        void ExecuteSqlBulkCopy<T>(string tableName, List<T> models, int batchSize = 1000, int? bulkCopyTimeout = null);
        Task ExecuteSqlBulkCopyAsync(string tableName, DataTable table, int batchSize = 1000, int? bulkCopyTimeout = null);
        Task ExecuteSqlBulkCopyAsync(string tableName, DataTable table, int batchSize = 1000, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, bool isBeginTransaction = true, int? bulkCopyTimeout = null);
        Task ExecuteSqlBulkCopyAsync<T>(string tableName, List<T> models, int batchSize = 1000, int? bulkCopyTimeout = null);
        SqlConnection GetSqlConnection();
    }

    public interface IDapperSqlServerClient<TDbConnectionHandler> : IDapperSqlServerClient, IDapperClient<TDbConnectionHandler> where TDbConnectionHandler : IDbConnectionHandler
    {

    }
}
