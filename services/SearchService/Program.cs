

using System.Text.RegularExpressions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Typesense;
using Typesense.Setup;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var typesenseUri = builder.Configuration["services:typesense:typesense:0"];

if(string.IsNullOrEmpty(typesenseUri))
{
    throw new Exception("Typesense URI is not configured. Please set the 'services:typesense:typesense:0' configuration value.");   
}
var typesenseApiKey = builder.Configuration["typesense-api-key"];
if(string.IsNullOrEmpty(typesenseApiKey))
{
    throw new Exception("Typesense API key is not configured. Please set the 'typesense-api-key' configuration value.");   
}   

var uri = new Uri(typesenseUri);
builder.Services.AddTypesenseClient(config =>
{
    config.ApiKey = typesenseApiKey;
    config.Nodes = new List<Typesense.Setup.Node> { new(uri.Host, uri.Port.ToString(), uri.Scheme) };
});

builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(builder.Environment.ApplicationName))
        .AddSource("Wolverine");
});

builder.Host.UseWolverine(options =>
{
    options.UseRabbitMqUsingNamedConnection("messaging").AutoProvision();
    options.ListenToRabbitQueue("questions.search", cfg =>
    {
        cfg.BindExchange("questions");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapDefaultEndpoints();

app.MapGet("/search", async (string query, ITypesenseClient client, ILogger<Program> logger) =>
{
    string? tag = null; 
    var tagMatch = Regex.Match(query, @"\[(.*?)\]");
    if (tagMatch.Success)
    {
        tag = tagMatch.Groups[1].Value;
        query = query.Replace(tagMatch.Value, "").Trim();
    }

    var searchParameters = new SearchParameters(query, "title,content")
    {
        FilterBy = tag != null ? $"tags:=[{tag}]" : null,
        SortBy = "createdAt:desc"
    };

    try
    {
        var searchResults = await client.Search<SearchService.Models.SearchQuestion>("questions", searchParameters);
        return Results.Ok(searchResults.Hits.Select(hit => hit.Document));
    }
    catch (TypesenseApiException ex)
    {
        logger.LogError(ex, "Typesense API error during search for query {Query}", query);
        return Results.Problem("An error occurred while searching. Please try again later.");
    }
});

app.MapGet("/search/similar-titles", async (string query, ITypesenseClient client, ILogger<Program> logger) =>
{
    var searchParameters = new SearchParameters(query, "title")
    {
        SortBy = "createdAt:desc"
    };

    try
    {
        var searchResults = await client.Search<SearchService.Models.SearchQuestion>("questions", searchParameters);
        return Results.Ok(searchResults.Hits.Select(hit => hit.Document));
    }
    catch (TypesenseApiException ex)
    {
        logger.LogError(ex, "Typesense API error during similar-titles search for query {Query}", query);
        return Results.Problem("An error occurred while searching. Please try again later.");
    }
});

using var scope = app.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<ITypesenseClient>();
await SearchService.Data.SearchInitializer.EnsureIndexExists(client);

app.Run();