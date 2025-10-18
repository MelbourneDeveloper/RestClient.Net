#pragma warning disable CS8509

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Generates C# extension methods from OpenAPI specifications.
/// This uses the Microsoft.OpenApi library to parse OpenAPI documents and generate code https://github.com/microsoft/OpenAPI.NET.
/// See the tests here https://github.com/microsoft/OpenAPI.NET/tree/main/test/Microsoft.OpenApi.Tests to see how the API works.
/// </summary>
public static class OpenApiCodeGenerator
{
    /// <summary>Generates code from an OpenAPI document.</summary>
    /// <param name="openApiContent">The OpenAPI document content (JSON or YAML).</param>
    /// <param name="namespace">The namespace for generated code.</param>
    /// <param name="className">The class name for extension methods.</param>
    /// <param name="outputPath">The directory path where generated files will be saved.</param>
    /// <param name="baseUrlOverride">Optional base URL override. Use this when the OpenAPI spec has a relative server URL.</param>
    /// <param name="jsonNamingPolicy">JSON naming policy (camelCase, PascalCase, snake_case).</param>
    /// <param name="caseInsensitive">Enable case-insensitive JSON deserialization.</param>
    /// <returns>A Result containing either the generated code or an error message with exception details.</returns>
#pragma warning disable CA1054
    public static Outcome.Result<GeneratorResult, string> Generate(
        string openApiContent,
        string @namespace,
        string className,
        string outputPath,
        string? baseUrlOverride = null,
        string jsonNamingPolicy = "camelCase",
        bool caseInsensitive = true
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
                return new GeneratorError("Error parsing OpenAPI: Document is null");
            }

            var document = readResult.Document;

            var urlResult = UrlParser.GetBaseUrlAndPath(document, baseUrlOverride);

            return urlResult switch
            {
                OkUrl(var (baseUrl, basePath)) => GenerateCodeFiles(
                    document,
                    @namespace,
                    className,
                    outputPath,
                    baseUrl,
                    basePath,
                    jsonNamingPolicy,
                    caseInsensitive
                ),
                ErrorUrl(var error) => new GeneratorError($"Error: {error}"),
            };
        }
        catch (Exception ex)
        {
            return new GeneratorError(
                $"Exception during code generation: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}"
            );
        }
    }

    private static GeneratorOk GenerateCodeFiles(
        OpenApiDocument document,
        string @namespace,
        string className,
        string outputPath,
        string baseUrl,
        string basePath,
        string jsonNamingPolicy,
        bool caseInsensitive
    )
    {
        var (extensionMethods, typeAliases) = ExtensionMethodGenerator.GenerateExtensionMethods(
            document,
            @namespace,
            className,
            baseUrl,
            basePath,
            jsonNamingPolicy,
            caseInsensitive
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

        return new GeneratorOk(new GeneratorResult(extensionMethods, models));
    }
}
