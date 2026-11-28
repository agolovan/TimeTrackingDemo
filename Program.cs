using Microsoft.EntityFrameworkCore;
using TimeTrackingDemo.Data;
using TimeTrackingDemo.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Connection string for SQL Server
var connectionString = builder.Configuration.GetConnectionString("TimeTracking")
    ?? throw new InvalidOperationException(
        "Connection string 'TimeTracking' was not found.");

builder.Services.AddDbContext<TimeTrackingDemoContext>(options =>
    options.UseSqlServer(connectionString));

// Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException(
        "Connection string 'Redis' was not found.");
var redisInstanceName = builder.Configuration["Redis:InstanceName"]
    ?? throw new InvalidOperationException(
        "Redis instance name 'Redis:InstanceName' was not found.");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = redisInstanceName;
});

// Time Entry Validator
builder.Services.AddTransient<TimeEntryValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
