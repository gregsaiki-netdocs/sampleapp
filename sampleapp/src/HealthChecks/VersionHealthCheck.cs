using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace HealthChecks;

public class VersionHealthCheck : IHealthCheck
{
    public const string NAME = "version";

    private ILogger<VersionHealthCheck> _logger;

    public VersionHealthCheck(ILogger<VersionHealthCheck> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        JsonNode jd;
        try
        {
            await using (FileStream fs = File.OpenRead("./version.json"))
            {
                jd = JsonNode.Parse(fs) ?? throw new JsonException();
            }
        }
        catch (Exception ex) when (ex is FileNotFoundException
                                || ex is IOException
                                || ex is JsonException
                                )
        {
            // noop since version.json is bad. For local builds this is expected.
            // If this happens in production, need to pull the image and see what is
            // wrong with version.json
            jd = new JsonObject();
            jd["version"] = "local";
        }
        var extra = new Dictionary<string, object>();
        foreach(var node in jd.AsObject())
        {
            extra[node.Key] = node.Value?.ToString() ?? string.Empty;
        }
        return HealthCheckResult.Healthy("version file", extra);
    }
}
