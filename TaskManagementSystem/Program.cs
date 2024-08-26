using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repository.Interface;
using TaskManagementSystem.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDBSettings>(
 builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
 new MongoClient(sp.GetRequiredService<IOptions<MongoDBSettings>>().Value.ConnectionString));

builder.Services.AddScoped<IMongoDatabase>(sp =>
 sp.GetRequiredService<IMongoClient>()
    .GetDatabase(sp.GetRequiredService<IOptions<MongoDBSettings>>().Value.DatabaseName));

 
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IListRepository, ListRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6380"; // Redis server URL (adjust as needed)
    options.InstanceName = "TaskManagement_"; // Optional prefix for cache keys
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
