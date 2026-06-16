using System.Net.Sockets;
using JasperFx.Descriptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Common;

/// <summary>
/// Shared Wolverine + RabbitMQ configuration helpers so every service wires up
/// messaging the same way against the Aspire-provided "messaging" connection.
/// </summary>
public static class WolverineExtensions
{
    /// <summary>
    /// The Aspire connection name for the RabbitMQ broker.
    /// </summary>
    public const string MessagingConnectionName = "messaging";


    /// <summary>
    /// Registers Wolverine + RabbitMQ messaging. Runs at configuration time
    /// (before <c>Build()</c>), so it takes no logger — there is no DI container yet.
    /// Pair it with <see cref="WaitForRabbitMqAsync"/> after the host is built.
    /// </summary>
    public static IHostApplicationBuilder AddWolverineMessaging(
        this IHostApplicationBuilder builder,
        Action<WolverineOptions> configureMessaging)
    {
        builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(builder.Environment.ApplicationName))
                .AddSource("Wolverine");
        });

        builder.UseWolverine(options =>
        {
            options.UseRabbitMqUsingNamedConnection(MessagingConnectionName)
                .AutoProvision()
                .DeclareExchange("questions");

            configureMessaging(options);
        });

        return builder;
    }

    /// <summary>
    /// Waits (with exponential backoff) until the RabbitMQ broker is reachable.
    /// Call this after <c>Build()</c> and before <c>Run()</c>, passing a logger
    /// resolved from the built host's DI — no bootstrap logger needed.
    /// </summary>
    public static async Task WaitForRabbitMqAsync(this IHost app, ILogger logger)
    {
        var configuration = app.Services.GetRequiredService<IConfiguration>();

        var endpoint = configuration.GetConnectionString(MessagingConnectionName)
            ?? throw new InvalidOperationException("RabbitMQ connection string is not configured.");

        var retryPolicy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(exception,
                        "RabbitMQ connection failed. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.",
                        timeSpan, retryCount);
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(endpoint)
            };
            await using var connection = await factory.CreateConnectionAsync();
        });
    }
}
