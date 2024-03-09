using Dapper;
using MuTian.Dapper.Interfaces;
using System.Data;

namespace MuTian.Dapper
{
    public class DapperClient<TDbConnectionHandler> : DapperClient, IDapperClient<TDbConnectionHandler> where TDbConnectionHandler : IDbConnectionHandler
    {
        public DapperClient(TDbConnectionHandler dbConnectionHandler) : base(dbConnectionHandler)
        {
            
        }
    }

    public class DapperClient : IDapperClient
    {
        public IDbConnection DbConnection { get; private set; }
        
        public DapperClient()
        {
            DbConnection = ConnectionFactory.CreateConnection(ConnectionFactory.GetConnectionSettings("DB:DefaultDbConfig"));
        }

        public DapperClient(IDbConnectionHandler dbConnectionHandler)
        {
            DbConnection = ConnectionFactory.CreateConnection(dbConnectionHandler.GetConnectionSettings());
        }

        public void ConnectionOpen()
        {
            if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.DbConnection.Open();
            }
        }

        public void ConnectionClose()
        {
            if (this.DbConnection.State != ConnectionState.Closed)
            {
                this.DbConnection.Close();
            }
        }

        #region sync
        public int Execute(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.DbConnection.Execute(sql, param, transaction, commandTimeout, commandType);
        }

        public IDataReader ExecuteReader(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.DbConnection.ExecuteReader(sql, param, transaction, commandTimeout, commandType);
        }

        public DataTable ExecuteDataTable(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var reader = this.ExecuteReader(sql, param, null, null, commandType);
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }

        public DataSet ExecuteDataSet(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var reader = this.ExecuteReader(sql, param, null, null, commandType);
            DataSet ds = new DataSet();
            int i = 0;
            while (!reader.IsClosed)
            {
                ds.Tables.Add("Table" + (i + 1));
                ds.Tables[i].Load(reader);
                i++;
            }
            return ds;
        }

        public IEnumerable<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public SqlMapper.GridReader QueryMultiple(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryMultiple(sql, param, transaction, commandTimeout, commandType);
        }

        public T? QueryFirstOrDefault<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QueryFirst<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryFirst<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T QuerySingle<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QuerySingle<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T? QuerySingleOrDefault<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QuerySingleOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public T? ExecuteScalar<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public object? ExecuteScalar(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
        }
        #endregion

        #region async
        public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<object?> ExecuteScalarAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public Task<IDataReader> ExecuteReaderAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<DataTable> ExecuteDataTableAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var reader = await this.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType);
            DataTable table = new DataTable();
            table.Load(reader);
            return table;
        }

        public async Task<DataSet> ExecuteDataSetAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var reader = await this.ExecuteReaderAsync(sql, param, null, null, commandType);
            DataSet ds = new DataSet();
            int i = 0;
            while (!reader.IsClosed)
            {
                ds.Tables.Add("Table" + (i + 1));
                ds.Tables[i].Load(reader);
                i++;
            }
            return ds;
        }
        #endregion
    }
}
