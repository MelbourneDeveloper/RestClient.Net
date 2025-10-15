using Microsoft.OpenApi.Models;
using Outcome;

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Parses base URLs and paths from OpenAPI documents.</summary>
internal static class UrlParser
{
    /// <summary>Gets the base URL and path from an OpenAPI document.</summary>
    /// <param name="document">The OpenAPI document.</param>
    /// <param name="baseUrlOverride">Optional base URL override.</param>
    /// <returns>A result containing the base URL and path, or an error message.</returns>
    public static Result<(string BaseUrl, string BasePath), string> GetBaseUrlAndPath(
        OpenApiDocument document,
        string? baseUrlOverride
    )
    {
        var server = document.Servers.FirstOrDefault();

        if (server == null || string.IsNullOrWhiteSpace(server.Url))
        {
            return Result<(string, string), string>.Failure(
                "OpenAPI document must specify at least one server with a valid URL. "
                    + "Add a 'servers' section to your OpenAPI spec with a complete URL (e.g., https://api.example.com)."
            );
        }

        var fullUrl = server.Url;

        // If the URL is relative (e.g., "/api/v3"), require override
        if (fullUrl.StartsWith('/'))
        {
            return string.IsNullOrWhiteSpace(baseUrlOverride)
                ? Result<(string, string), string>.Failure(
                    $"Server URL '{fullUrl}' is relative. "
                        + "OpenAPI server URL must be an absolute URL with protocol and host (e.g., https://api.example.com/api/v3), "
                        + "or you must provide a baseUrlOverride parameter when calling Generate()."
                )
                : new Result<(string, string), string>.Ok<(string, string), string>(
                    (baseUrlOverride!, fullUrl.TrimEnd('/'))
                );
        }

        // Parse the URL to separate base URL (protocol + host) from base path
        if (!Uri.TryCreate(fullUrl, UriKind.Absolute, out var uri))
        {
            return Result<(string, string), string>.Failure(
                $"Server URL '{fullUrl}' is not a valid absolute URL. "
                    + "URL must include protocol and host (e.g., https://api.example.com)."
            );
        }

        // Check if it's a valid http/https URL (not file://)
        if (uri.Scheme is not "http" and not "https")
        {
            return Result<(string, string), string>.Failure(
                $"Server URL '{fullUrl}' has unsupported scheme '{uri.Scheme}'. "
                    + "Only http and https schemes are supported."
            );
        }

        var baseUrl = baseUrlOverride ?? $"{uri.Scheme}://{uri.Authority}";
        var basePath = uri.AbsolutePath.TrimEnd('/');
        return new Result<(string, string), string>.Ok<(string, string), string>(
            (baseUrl, basePath)
        );
    }
}
