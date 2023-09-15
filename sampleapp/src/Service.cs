using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using sampleapp.Clock;
using sampleapp.Endpoints;
using HealthChecks;

namespace Initialization;

internal class Service
{
    /// <summary>
    /// Logger
    /// </summary>
    private ILogger<Service> _log;

    public Service(ILogger<Service> log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }


    /// <summary>
    /// Register additional services in the dependency injection system.
    /// </summary>
    /// <param name="hbContext"></param>
    /// <param name="services">Service collection to add services to</param>
    /// <param name="healthChecks">Called to add health checks</param>
    internal static void ConfigureServices(HostBuilderContext hbContext, IServiceCollection services, IHealthChecksBuilder? healthChecks)
    {
        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<CurrentDocumentModificationNumberProvider>();
    }

    /// <summary>
    /// Map service endpoints
    /// </summary>
    /// <param name="app"></param>
    internal static void MapServiceEndpoints(WebApplication app)
    {
        CurrentDocumentModificationNumberProvider endpoint = app.Services.GetService<CurrentDocumentModificationNumberProvider>() ?? throw new InvalidOperationException();

        // Get current document modification number based on UTC
        app.MapGet("/utc", () => endpoint.GetUTC())
        .WithName("utc")
        .WithDisplayName("Universal Time")
        .WithDisplayName("Current Universal Time")
        .WithDescription("Current universal timestamp")
        .Produces<long>(StatusCodes.Status200OK);

        // Get current document modification number based on UTC
        app.MapGet("/", () => endpoint.GetUTC())
        .WithDescription("Current universal timestamp")
        .Produces<long>(StatusCodes.Status200OK);

        // Get current document modification number based on an offset from UTC
        app.MapGet("local/{offset}", ([FromRoute] long offset) => endpoint.GetLocalOffset(offset) )
        .WithName("local")
        .WithDisplayName("Local Offset")
        .WithDescription("Offset from current universal time")
        .WithSummary("Converts from universal time to the provided local offset time")
        .Produces<long>(StatusCodes.Status200OK);

        // Get current document modification number based on an offset from UTC but which is slowing migrating to UTC
        app.MapGet("migrating/{offset}/{begin}/{end}", ([FromRoute] long offset, [FromRoute] long begin, [FromRoute] long end) => endpoint.GetMigratingOffset(offset, begin, end))
        .WithName("migrate")
        .WithDisplayName("Migrating Offset")
        .WithDescription("Migrate from a local offset to universal time")
        .WithSummary("Using the provided offset and migration period returns the appropriate adjusted timestamp.")
        .Produces<long>(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Outputs healthcheck status in custom format
    /// </summary>
    /// <param name="context"></param>
    /// <param name="healthReport"></param>
    /// <returns></returns>
    internal static Task WriteHealthCheckResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            var entry = healthReport.Entries[VersionHealthCheck.NAME];
            foreach (var item in entry.Data)
            {
                jsonWriter.WritePropertyName(item.Key);

                JsonSerializer.Serialize(jsonWriter, item.Value,
                    item.Value?.GetType() ?? typeof(object));
            }
            foreach (var healthReportEntry in healthReport.Entries)
            {
                if(healthReportEntry.Key == VersionHealthCheck.NAME)
                {
                    // Skip the version one
                    continue;
                }
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject(); // end entry object
            }

            jsonWriter.WriteEndObject(); // end root
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}