#pragma warning disable CS8509

using Outcome;

namespace RestClient.Net.McpGenerator;

/// <summary>Generates MCP server code from OpenAPI specifications.</summary>
public static class McpServerGenerator
{
    /// <summary>Generates MCP server tools code from an OpenAPI document.</summary>
    /// <param name="openApiContent">The OpenAPI document content (JSON or YAML).</param>
    /// <param name="namespace">The namespace for generated MCP tools.</param>
    /// <param name="serverName">The MCP server name.</param>
    /// <param name="extensionsNamespace">The namespace of the pre-generated extensions.</param>
    /// <param name="extensionsClassName">The class name of the pre-generated extensions.</param>
    /// <returns>A Result containing the generated C# code or error message.</returns>
#pragma warning disable CA1054
    public static Result<string, string> Generate(
        string openApiContent,
        string @namespace,
        string serverName,
        string extensionsNamespace,
        string extensionsClassName
    )
#pragma warning restore CA1054
    {
        try
        {
            var settings = new OpenApiReaderSettings();
            settings.AddYamlReader();

            var readResult = OpenApiDocument.Parse(openApiContent, settings: settings);

            if (readResult.Document == null)
            {
                return new Result<string, string>.Error<string, string>("Error parsing OpenAPI: Document is null");
            }

            var document = readResult.Document;

            return new Result<string, string>.Ok<string, string>(
                McpToolGenerator.GenerateTools(
                    document,
                    @namespace,
                    serverName,
                    extensionsNamespace,
                    extensionsClassName
                )
            );
        }
        catch (Exception ex)
        {
            return new Result<string, string>.Error<string, string>(
                $"Exception during code generation: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
    }
}
