# Simple package based on dapper

The project is to provide a simple extension to dapper, combining IOC and filters, and support for tenant database switching

**You can use nuget to search for the MuTian.Dapper package to use**

## Examples:

```csharp
builder.Services.AddTransient<IDapperClient, DapperClient>();
builder.Services.AddTransient(typeof(IDapperClient<>), typeof(DapperClient<>));
builder.Services.AddTransient<IDapperSqlServerClient, DapperSqlServerClient>();
builder.Services.AddTransient(typeof(IDapperSqlServerClient<>), typeof(DapperSqlServerClient<>));

IDapperClient dapperClient;
dapperClient = ServiceProvider.GetRequiredService<IDapperSqlServerClient>();
dapperClient.ExecuteDataSet(
                "select * from users;" +
                "select * from users;");

// Transaction
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

//SqlBulkCopy
var dapperSqlServerClient = (IDapperSqlServerClient)dapperClient;
List<User> userList = new List<User>();
for (int i = 0; i < 10; i++)
{
    userList.Add(new User ());
}
dapperSqlServerClient.ExecuteSqlBulkCopy("users", userList.ToDataTable(), 10, 120);
dapperSqlServerClient.ExecuteSqlBulkCopy("users", userList, 10, 120);
```

**See Test Cases for more information**