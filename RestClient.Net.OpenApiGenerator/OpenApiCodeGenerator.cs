using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Validations;
using ErrorUrl = Outcome.Result<(string, string), string>.Error<(string, string), string>;
using GeneratorError = Outcome.Result<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>.Error<RestClient.Net.OpenApiGenerator.GeneratorResult, string>;
using GeneratorOk = Outcome.Result<RestClient.Net.OpenApiGenerator.GeneratorResult, string>.Ok<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>;
using OkUrl = Outcome.Result<(string, string), string>.Ok<(string, string), string>;

#pragma warning disable CS8509

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
    /// <returns>A Result containing either the generated code or an error message with exception details.</returns>
#pragma warning disable CA1054
    public static Outcome.Result<GeneratorResult, string> Generate(
        string openApiContent,
        string @namespace,
        string className,
        string outputPath,
        string? baseUrlOverride = null
    )
#pragma warning restore CA1054
    {
        try
        {
            var document = new OpenApiStringReader(
                new OpenApiReaderSettings
                {
                    ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences,
                    RuleSet = ValidationRuleSet.GetDefaultRuleSet(),
                }
            ).Read(openApiContent, out var diagnostic);

            if (diagnostic.Errors.Count > 0)
            {
                var errors = string.Join(", ", diagnostic.Errors.Select(e => e.Message));
                return new GeneratorError($"Error parsing OpenAPI: {errors}");
            }

            if (document == null)
            {
                return new GeneratorError("Error parsing OpenAPI: Document is null");
            }

            var urlResult = UrlParser.GetBaseUrlAndPath(document, baseUrlOverride);

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

        return new GeneratorOk(new GeneratorResult(extensionMethods, models));
    }
}
