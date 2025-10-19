using System.Text.RegularExpressions;
using Outcome;

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Parses base URLs and paths from OpenAPI documents.</summary>
public static partial class UrlParser
{
    [GeneratedRegex(@"\{[^}]+\}")]
    private static partial Regex TemplateVariableRegex();

    /// <summary>Gets the base URL and path from an OpenAPI document.</summary>
    /// <param name="document">The OpenAPI document.</param>
    /// <param name="baseUrlOverride">Optional base URL override.</param>
    /// <returns>A result containing the base URL and path, or an error message.</returns>
    public static Result<(string BaseUrl, string BasePath), string> GetBaseUrlAndPath(
        OpenApiDocument document,
        string? baseUrlOverride
    )
    {
        var server = document.Servers?.FirstOrDefault();

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
                : new OkUrl((baseUrlOverride!, fullUrl.TrimEnd('/')));
        }

        // Handle URLs with template variables (e.g., https://{region}.example.com)
        var urlForParsing = fullUrl;
        var hasTemplateVariables = fullUrl.Contains('{', StringComparison.Ordinal);
        if (hasTemplateVariables)
        {
            // Replace template variables with placeholder for parsing
            urlForParsing = TemplateVariableRegex().Replace(fullUrl, "placeholder");
        }

        // Parse the URL to separate base URL (protocol + host) from base path
        if (!Uri.TryCreate(urlForParsing, UriKind.Absolute, out var uri))
        {
            // If URL is invalid but override is provided, use override
            return !string.IsNullOrWhiteSpace(baseUrlOverride)
                ? new OkUrl((baseUrlOverride!, string.Empty))
                : Result<(string, string), string>.Failure(
                    $"Server URL '{fullUrl}' is not a valid absolute URL. "
                        + "URL must include protocol and host (e.g., https://api.example.com), "
                        + "or you must provide a baseUrlOverride parameter when calling Generate()."
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

        // If there are template variables and no override, use the full URL as base
        if (hasTemplateVariables && string.IsNullOrWhiteSpace(baseUrlOverride))
        {
            // Extract path from parsed URL, but use original fullUrl for base
            var basePath = uri.AbsolutePath.TrimEnd('/');
            return new OkUrl(
                (
                    fullUrl
                        .Replace(uri.AbsolutePath, string.Empty, StringComparison.Ordinal)
                        .TrimEnd('/'),
                    basePath
                )
            );
        }

        // If override is provided, parse it to separate baseUrl and basePath
        if (!string.IsNullOrWhiteSpace(baseUrlOverride))
        {
            if (Uri.TryCreate(baseUrlOverride, UriKind.Absolute, out var overrideUri))
            {
                var overrideBaseUrl = $"{overrideUri.Scheme}://{overrideUri.Authority}";
                var overrideBasePath = overrideUri.AbsolutePath.TrimEnd('/');
                return new OkUrl((overrideBaseUrl, overrideBasePath));
            }
        }

        var baseUrl = $"{uri.Scheme}://{uri.Authority}";
        var basePath2 = uri.AbsolutePath.TrimEnd('/');
        return new OkUrl((baseUrl, basePath2));
    }
}
