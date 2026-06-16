using System.Net.Sockets;
using Common;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using QuestionService.Data;
using QuestionService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.AddServiceDefaults();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<TagService>();
builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer(serviceName: "keycloak", realm: "overflow", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "overflow";
    });

builder.AddNpgsqlDbContext<QuestionDbContext>("questionDb");

builder.AddWolverineMessaging(options =>
{
    options.PublishAllMessages().ToRabbitExchange("questions");
    options.ApplicationAssembly = typeof(Program).Assembly;
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

await app.WaitForRabbitMqAsync(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapDefaultEndpoints();

using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<QuestionDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while seeding the database.");
}

app.Run();