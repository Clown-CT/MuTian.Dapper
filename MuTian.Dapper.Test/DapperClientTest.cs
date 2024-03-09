using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuTian.Dapper.Extesions;
using MuTian.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuTian.Dapper.Test
{
    public class DapperClientTest
    {
        IConfiguration Configuration { get; }

        IServiceProvider ServiceProvider { get; }
        public DapperClientTest()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            ServiceCollection services = new ServiceCollection();
            services.AddTransient<IDapperClient, DapperClient>();
            services.AddTransient(typeof(IDapperClient<>), typeof(DapperClient<>));
            services.AddTransient<IDapperSqlServerClient, DapperSqlServerClient>();
            services.AddTransient(typeof(IDapperSqlServerClient<>), typeof(DapperSqlServerClient<>));
            services.AddTransient<MainDbConnectionHandler>();
            ServiceProvider = services.BuildServiceProvider();
        }


        [Fact]
        public void DapperSqlserverQuery()
        {
            IDapperClient dapperClient;
            //dapperClient = ServiceProvider.GetRequiredService<IDapperClient>();
            //dapperClient = ServiceProvider.GetRequiredService<IDapperClient<MainDbConnectionHandler>>();
            dapperClient = ServiceProvider.GetRequiredService<IDapperSqlServerClient>();
            //dapperClient = ServiceProvider.GetRequiredService<IDapperSqlServerClient<MainDbConnectionHandler>>();
            Assert.True(dapperClient.Execute("INSERT INTO [dbo].[users]([userName],[age])VALUES (@userName,@age)", new User ()) == 1);

            if (dapperClient is IDapperSqlServerClient)
            {
                var executeSqlBulkCopyBeforeCount = dapperClient.ExecuteScalar<int>("select count(*) from users");
                var dapperSqlServerClient = (IDapperSqlServerClient)dapperClient;
                List<User> userList = new List<User>();
                for (int i = 0; i < 10; i++)
                {
                    userList.Add(new User ());
                }
                dapperSqlServerClient.ExecuteSqlBulkCopy("users", userList.ToDataTable(), 10, 120);
                dapperSqlServerClient.ExecuteSqlBulkCopy("users", userList, 10, 120);
                var executeSqlBulkCopyAfterCount = dapperClient.ExecuteScalar<int>("select count(*) from users");
                Assert.True(executeSqlBulkCopyAfterCount > executeSqlBulkCopyBeforeCount);
            }

            Assert.True(dapperClient.ExecuteDataSet(
                "select * from users;" +
                "select * from users;").Tables.Count == 2);

            Assert.True(dapperClient.ExecuteDataTable("select * from users;").Rows.Count > 0);

            DataTable dt = new DataTable();
            dt.Load(dapperClient.ExecuteReader("select * from users;"));
            Assert.True(dt.Rows.Count > 0);

            Assert.NotNull(dapperClient.ExecuteScalar("select top 1 userName from users;"));
            Assert.NotNull(dapperClient.ExecuteScalar<string>("select top 1 userName from users;"));

            SqlMapper.GridReader gridReader = dapperClient.QueryMultiple(
                "select * from users;" +
                "select * from users;");
            Assert.True(gridReader.Read<User>().Count() > 0);
            Assert.True(gridReader.Read<User>().Count() > 0);

            Assert.Equal(dapperClient.QueryFirst<User>("select top 1 * from users order by age;").Age, dapperClient.QueryFirst<User>("select * from users order by age;").Age);

            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                dapperClient.QueryFirst<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") });
            });

            Assert.NotNull(dapperClient.QueryFirstOrDefault<User>("select top 1 * from users where age > @age;", new { Age = 0 }));
            Assert.Null(dapperClient.QueryFirstOrDefault<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") }));

            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                dapperClient.QuerySingle<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") });
            });
            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                dapperClient.QuerySingle<User>("select * from users;");
            });

            Assert.Null(dapperClient.QuerySingleOrDefault<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") }));

            #region Transaction
            dapperClient.ConnectionOpen();
            var dbTransaction = dapperClient.DbConnection.BeginTransaction();
            try
            {
                Assert.True(dapperClient.Execute("INSERT INTO [dbo].[users]([userName],[age])VALUES (@userName,@age)", new User(), dbTransaction) == 1);

                Assert.True(dapperClient.Execute("INSERT INTO [dbo].[users]([userName],[age])VALUES (@userName,@age),(@userName,@age)", new List<User>()
                {
                    new User (),
                    new User ()
                }) == 2);
                dbTransaction.Commit();
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
            }
            finally
            {
                dapperClient.ConnectionClose();
            }
            #endregion
        }

        [Fact]
        public async Task DapperSqlserverQueryAsync()
        {
            IDapperClient dapperClient;
            //dapperClient = ServiceProvider.GetRequiredService<IDapperClient>();
            //dapperClient = ServiceProvider.GetRequiredService<IDapperClient<MainDbConnectionHandler>>();
            dapperClient = ServiceProvider.GetRequiredService<IDapperSqlServerClient>();
            //dapperClient = ServiceProvider.GetRequiredService<IDapperSqlServerClient<MainDbConnectionHandler>>();
            Assert.True(await dapperClient.ExecuteAsync("INSERT INTO [dbo].[users]([userName],[age])VALUES (@userName,@age)", new User ()) == 1);

            if (dapperClient is IDapperSqlServerClient)
            {
                var executeSqlBulkCopyBeforeCount = await dapperClient.ExecuteScalarAsync<int>("select count(*) from users");
                var dapperSqlServerClient = (IDapperSqlServerClient)dapperClient;
                List<User> userList = new List<User>();
                for (int i = 0; i < 10; i++)
                {
                    userList.Add(new User ());
                }
                await dapperSqlServerClient.ExecuteSqlBulkCopyAsync("users", userList.ToDataTable(), 10, 120);
                await dapperSqlServerClient.ExecuteSqlBulkCopyAsync("users", userList, 10, 120);
                var executeSqlBulkCopyAfterCount = await dapperClient.ExecuteScalarAsync<int>("select count(*) from users");
                Assert.True(executeSqlBulkCopyAfterCount > executeSqlBulkCopyBeforeCount);
            }

            Assert.True((await dapperClient.ExecuteDataSetAsync(
                "select * from users;" +
                "select * from users;")).Tables.Count == 2);

            Assert.True((await dapperClient.ExecuteDataTableAsync("select * from users;")).Rows.Count > 0);

            DataTable dt = new DataTable();
            dt.Load(await dapperClient.ExecuteReaderAsync("select * from users;"));
            Assert.True(dt.Rows.Count > 0);

            Assert.NotNull(await dapperClient.ExecuteScalarAsync("select top 1 userName from users;"));
            Assert.NotNull(await dapperClient.ExecuteScalarAsync<string>("select top 1 userName from users;"));

            SqlMapper.GridReader gridReader = await dapperClient.QueryMultipleAsync(
                "select * from users;" +
                "select * from users;");
            Assert.True(gridReader.Read<User>().Count() > 0);
            Assert.True(gridReader.Read<User>().Count() > 0);

            Assert.Equal(
                (await dapperClient.QueryFirstAsync<User>("select top 1 * from users order by age;")).Age,
                (await dapperClient.QueryFirstAsync<User>("select * from users order by age;")).Age);

            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                await dapperClient.QueryFirstAsync<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") });
            });

            Assert.NotNull(await dapperClient.QueryFirstOrDefaultAsync<User>("select top 1 * from users where age > @age;", new { Age = 0 }));
            Assert.Null(await dapperClient.QueryFirstOrDefaultAsync<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") }));

            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                await dapperClient.QuerySingleAsync<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") });
            });
            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                await dapperClient.QuerySingleAsync<User>("select * from users;");
            });

            Assert.Null(await dapperClient.QuerySingleOrDefaultAsync<User>("select top 1 * from users where userName=@userName;", new { userName = Guid.NewGuid().ToString("N") }));

            #region Transaction
            dapperClient.ConnectionOpen();
            var dbTransaction = dapperClient.DbConnection.BeginTransaction();
            try
            {
                Assert.True(await dapperClient.ExecuteAsync("INSERT INTO [dbo].[users]([userName],[age])VALUES (@userName,@age)", new User (), dbTransaction) == 1);

                Assert.True(await dapperClient.ExecuteAsync("INSERT INTO [dbo].[users]([userName],[age])VALUES (@userName,@age),(@userName,@age)", new List<User>()
                {
                    new User (),
                    new User ()
                }) == 2);
                dbTransaction.Commit();
            }
            catch (Exception)
            {
                dbTransaction.Rollback();
            }
            finally
            {
                dapperClient.ConnectionClose();
            }
            #endregion
        }
    }
}
