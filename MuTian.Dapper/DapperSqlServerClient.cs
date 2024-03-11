using Microsoft.Data.SqlClient;
using MuTian.Dapper.Extesions;
using MuTian.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MuTian.Dapper
{
    public class DapperSqlServerClient<TDbConnectionHandler> : DapperSqlServerClient, IDapperSqlServerClient<TDbConnectionHandler> where TDbConnectionHandler : IDbConnectionHandler
    {
        public DapperSqlServerClient(TDbConnectionHandler dbConnectionHandler) : base(dbConnectionHandler)
        {

        }
    }

    public class DapperSqlServerClient : DapperClient, IDapperSqlServerClient, IDatabaseCheck
    {
        public DapperSqlServerClient()
        {
            CheckConnectionDatabase();
        }
        public DapperSqlServerClient(IDbConnectionHandler dbConnectionHandler) : base(dbConnectionHandler)
        {
            CheckConnectionDatabase();
        }

        public void CheckConnectionDatabase()
        {
            if (!(base.DbConnection is SqlConnection))
            {
                throw new Exception("The database is not Sqlserver");
            }
        }

        public SqlConnection GetSqlConnection()
        {
            return (SqlConnection)this.DbConnection;
        }
        public void ExecuteSqlBulkCopy<T>(string tableName, List<T> models, int batchSize = 1000, int? bulkCopyTimeout = null)
        {
            ExecuteSqlBulkCopy(tableName, models.ToDataTable(), batchSize, SqlBulkCopyOptions.Default, bulkCopyTimeout: bulkCopyTimeout);
        }

        public void ExecuteSqlBulkCopy(string tableName, DataTable table, int batchSize = 1000, int? bulkCopyTimeout = null)
        {
            ExecuteSqlBulkCopy(tableName, table, batchSize, SqlBulkCopyOptions.Default, bulkCopyTimeout: bulkCopyTimeout);
        }

        public void ExecuteSqlBulkCopy(string tableName, DataTable table, int batchSize, SqlBulkCopyOptions sqlBulkCopyOptions, bool isBeginTransaction = true, int? bulkCopyTimeout = null)
        {
            base.ConnectionOpen();
            var conn = GetSqlConnection();
            var sqlTransaction = conn.BeginTransaction();
            SqlBulkCopy? sqlBulkCopy = null;
            try
            {
                if (isBeginTransaction)
                {
                    sqlBulkCopy = new SqlBulkCopy(conn, sqlBulkCopyOptions, sqlTransaction);
                }
                else
                {
                    sqlBulkCopy = new SqlBulkCopy(conn);
                }
                sqlBulkCopy.BatchSize = batchSize;
                if (bulkCopyTimeout != null)
                    sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.WriteToServer(table);
                if (isBeginTransaction)
                {
                    sqlTransaction.Commit();
                }
            }
            catch
            {
                if (isBeginTransaction)
                {
                    sqlTransaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (sqlBulkCopy != null)
                {
                    sqlBulkCopy.Close();
                }
                base.ConnectionClose();
            }
        }

        public Task ExecuteSqlBulkCopyAsync<T>(string tableName, List<T> models, int batchSize = 1000, int? bulkCopyTimeout = null)
        {
            return ExecuteSqlBulkCopyAsync(tableName, models.ToDataTable(), batchSize, SqlBulkCopyOptions.Default, bulkCopyTimeout: bulkCopyTimeout);
        }
        public Task ExecuteSqlBulkCopyAsync(string tableName, DataTable table, int batchSize = 1000, int? bulkCopyTimeout = null)
        {
            return ExecuteSqlBulkCopyAsync(tableName, table, batchSize, SqlBulkCopyOptions.Default, bulkCopyTimeout: bulkCopyTimeout);
        }

        public async Task ExecuteSqlBulkCopyAsync(string tableName, DataTable table, int batchSize, SqlBulkCopyOptions sqlBulkCopyOptions, bool isBeginTransaction = true, int? bulkCopyTimeout = null)
        {
            base.ConnectionOpen();
            var conn = GetSqlConnection();
            var sqlTransaction = (SqlTransaction)await conn.BeginTransactionAsync();
            SqlBulkCopy? sqlBulkCopy = null;
            try
            {
                if (isBeginTransaction)
                {
                    sqlBulkCopy = new SqlBulkCopy(conn, sqlBulkCopyOptions, sqlTransaction);
                }
                else
                {
                    sqlBulkCopy = new SqlBulkCopy(conn);
                }
                sqlBulkCopy.BatchSize = batchSize;
                if (bulkCopyTimeout != null)
                    sqlBulkCopy.BulkCopyTimeout = bulkCopyTimeout.Value;
                sqlBulkCopy.DestinationTableName = tableName;
                await sqlBulkCopy.WriteToServerAsync(table);
                if (isBeginTransaction)
                {
                    await sqlTransaction.CommitAsync();
                }
            }
            catch
            {
                if (isBeginTransaction)
                {
                    await sqlTransaction.RollbackAsync();
                }
                throw;
            }
            finally
            {
                if (sqlBulkCopy != null)
                {
                    sqlBulkCopy.Close();
                }
                base.ConnectionClose();
            }
        }
    }
}
