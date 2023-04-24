using Azure.Monitor.OpenTelemetry.Exporter;
using HotChocolate.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Playground;

var builder = WebApplication.CreateBuilder(args);

var applicationInsightsConnectionString = builder.Configuration.GetSection("ApplicationInsights").GetValue<string>("ConnectionString");

builder.Logging.ClearProviders();

builder.Services
    .AddOpenTelemetry()
        .ConfigureResource(builder => builder
            .AddService("Playground"))
        .WithTracing(builder => builder
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddSqlClientInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
            })
            .AddHotChocolateInstrumentation()
            .AddConsoleExporter()
            .AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = applicationInsightsConnectionString;
            }))
        .WithMetrics(builder => builder
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = applicationInsightsConnectionString;
            }))
        .StartWithHost();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Playground"));
        options.AddAzureMonitorLogExporter(options =>
        {
            options.ConnectionString = applicationInsightsConnectionString;
        });
    })
    .AddSimpleConsole();

builder.Services
    .AddAuthorization()

    .AddGraphQLServer()
        .AddErrorFilter<ErrorFilter>()
        .AddInstrumentation(o =>
        {
            o.RenameRootActivity = true;
            o.IncludeDocument = true;
            o.Scopes = ActivityScopes.All;
            o.RequestDetails = RequestDetails.All;
        })
        .AddQueryType<Query>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGraphQL();

app.Run();
