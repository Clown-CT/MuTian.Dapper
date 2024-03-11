using MuTian.Dapper;
using MuTian.Dapper.ApiTest;
using MuTian.Dapper.ApiTest.Filters;
using MuTian.Dapper.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IDapperClient, DapperClient>();
builder.Services.AddTransient(typeof(IDapperClient<>), typeof(DapperClient<>));
builder.Services.AddTransient<IDapperSqlServerClient, DapperSqlServerClient>();
builder.Services.AddTransient(typeof(IDapperSqlServerClient<>), typeof(DapperSqlServerClient<>));
builder.Services.AddScoped<TenantDbConnectionHandler>();
builder.Services.AddTransient<TenantConnectionActionFilterAttribute>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
