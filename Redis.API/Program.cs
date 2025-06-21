using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Redis.API;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = builder.Configuration;

var redisSecretPath = config["RedisSecretPath"];

var redisProvider = new RedisConnectionProvider(redisSecretPath);

builder.Services.AddSingleton(redisProvider);

builder.Services.AddHealthChecks().AddRedis(redisProvider.GetConnection(), name: "Redis", failureStatus: HealthStatus.Unhealthy);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/store", async (SetRequest req, RedisConnectionProvider provider) =>
{
    if (string.IsNullOrEmpty(req.Key) || req.Value == null)
        return Results.BadRequest("Chave e valor devem ser informados.");

    var db = provider.GetConnection().GetDatabase();
    bool result = await db.StringSetAsync(req.Key, req.Value);

    if (result)
        return Results.Ok($"Chave '{req.Key}' armazenada com sucesso.");
    else
        return Results.StatusCode(500);
}).WithName("StoreKeys")
.WithOpenApi();

app.MapGet("/keys", (RedisConnectionProvider provider) =>
{
    var server = provider.GetConnection().GetServer(provider.GetConnection().GetEndPoints()[0]);
    var keys = server.Keys(pattern: "*", pageSize: 1000);
    return Results.Ok(keys.Select(k => k.ToString()));
}).WithName("GetKeys")
.WithOpenApi();


app.MapHealthChecks("api/healthcheck");

app.Lifetime.ApplicationStopping.Register(() =>
{
    redisProvider.Dispose();
});

app.Run();

public record SetRequest(string Key, string Value);
