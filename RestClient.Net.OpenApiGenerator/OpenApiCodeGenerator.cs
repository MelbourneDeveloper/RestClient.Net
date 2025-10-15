using Microsoft.OpenApi.Readers;
using ErrorUrl = Outcome.Result<(string, string), string>.Error<(string, string), string>;
using OkUrl = Outcome.Result<(string, string), string>.Ok<(string, string), string>;

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Generates C# extension methods from OpenAPI specifications.</summary>
public static class OpenApiCodeGenerator
{
    /// <summary>Generates code from an OpenAPI document.</summary>
    /// <param name="openApiContent">The OpenAPI document content (JSON or YAML).</param>
    /// <param name="namespace">The namespace for generated code.</param>
    /// <param name="className">The class name for extension methods.</param>
    /// <param name="outputPath">The directory path where generated files will be saved.</param>
    /// <param name="baseUrlOverride">Optional base URL override. Use this when the OpenAPI spec has a relative server URL.</param>
    /// <param name="versionOverride">Optional OpenAPI version override (e.g., "3.0.2"). Use this when the spec declares the wrong version.</param>
    /// <returns>The generated code result.</returns>
#pragma warning disable CA1054
    public static GeneratorResult Generate(
        string openApiContent,
        string @namespace,
        string className,
        string outputPath,
        string? baseUrlOverride = null,
        string? versionOverride = null
    )
#pragma warning restore CA1054
    {
        // Apply version override if specified
        if (!string.IsNullOrEmpty(versionOverride))
        {
#pragma warning disable SYSLIB1045
            openApiContent = System.Text.RegularExpressions.Regex.Replace(
                openApiContent,
                @"^openapi:\s*[\d\.]+",
                $"openapi: {versionOverride}",
                System.Text.RegularExpressions.RegexOptions.Multiline
            );
#pragma warning restore SYSLIB1045
        }

        var reader = new OpenApiStringReader();
        var document = reader.Read(openApiContent, out var diagnostic);

        if (diagnostic.Errors.Count > 0)
        {
            var errors = string.Join(", ", diagnostic.Errors.Select(e => e.Message));
            return new GeneratorResult($"// Error parsing OpenAPI: {errors}", string.Empty);
        }

        if (document == null)
        {
            return new GeneratorResult("// Error parsing OpenAPI: Document is null", string.Empty);
        }

        var urlResult = UrlParser.GetBaseUrlAndPath(document, baseUrlOverride);

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        return urlResult switch
        {
            OkUrl(var (baseUrl, basePath)) => GenerateCodeFiles(
                document,
                @namespace,
                className,
                outputPath,
                baseUrl,
                basePath
            ),
            ErrorUrl(var error) => new GeneratorResult($"// Error: {error}", string.Empty),
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }

    private static GeneratorResult GenerateCodeFiles(
        Microsoft.OpenApi.Models.OpenApiDocument document,
        string @namespace,
        string className,
        string outputPath,
        string baseUrl,
        string basePath
    )
    {
        var (extensionMethods, typeAliases) = ExtensionMethodGenerator.GenerateExtensionMethods(
            document,
            @namespace,
            className,
            baseUrl,
            basePath
        );
        var models = ModelGenerator.GenerateModels(document, @namespace);

        _ = Directory.CreateDirectory(outputPath);

        var extensionsFile = Path.Combine(outputPath, $"{className}.g.cs");
        var modelsFile = Path.Combine(
            outputPath,
            $"{className.Replace("Extensions", "Models", StringComparison.Ordinal)}.g.cs"
        );
        var typeAliasesFile = Path.Combine(outputPath, "GlobalUsings.g.cs");

        File.WriteAllText(extensionsFile, extensionMethods);
        File.WriteAllText(modelsFile, models);
        File.WriteAllText(typeAliasesFile, typeAliases);

        return new GeneratorResult(extensionMethods, models);
    }
}
