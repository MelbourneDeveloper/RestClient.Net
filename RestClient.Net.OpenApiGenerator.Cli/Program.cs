using Microsoft.Extensions.DependencyInjection;
using RestClient.Net;
using RestClient.Net.OpenApiGenerator;
using ErrorString = Outcome.Result<string, Outcome.HttpError<string>>.Error<
    string,
    Outcome.HttpError<string>
>;
using ExceptionErrorString = Outcome.HttpError<string>.ExceptionError;
using OkString = Outcome.Result<string, Outcome.HttpError<string>>.Ok<
    string,
    Outcome.HttpError<string>
>;
using ResponseErrorString = Outcome.HttpError<string>.ErrorResponseError;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    PrintUsage();
    return 0;
}

var config = ParseArgs(args);
if (config is null)
{
    return 1;
}

await GenerateCode(config).ConfigureAwait(false);
return 0;

static void PrintUsage()
{
    Console.WriteLine("RestClient.Net OpenAPI Code Generator");
    Console.WriteLine("=====================================\n");
    Console.WriteLine("Usage:");
    Console.WriteLine("  restclient-generator [options]\n");
    Console.WriteLine("Options:");
    Console.WriteLine(
        "  -u, --openapi-url <url>       (Required) URL or file path to OpenAPI spec"
    );
    Console.WriteLine(
        "  -o, --output-path <path>      (Required) Output directory for generated code"
    );
    Console.WriteLine("  -n, --namespace <namespace>   The namespace (default: 'Generated')");
    Console.WriteLine("  -c, --class-name <name>       The class name (default: 'ApiExtensions')");
    Console.WriteLine("  -b, --base-url <url>          Optional base URL override");
    Console.WriteLine("  -v, --version <version>       OpenAPI version override (e.g., '3.1.0')");
    Console.WriteLine("  -h, --help                    Show this help message");
}

static Config? ParseArgs(string[] args)
{
    string? openApiUrl = null;
    string? outputPath = null;
    var namespaceName = "Generated";
    var className = "ApiExtensions";
    string? baseUrl = null;
    string? version = null;

    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "-u"
            or "--openapi-url":
                openApiUrl = GetNextArg(args, i++, "openapi-url");
                break;
            case "-o"
            or "--output-path":
                outputPath = GetNextArg(args, i++, "output-path");
                break;
            case "-n"
            or "--namespace":
                namespaceName = GetNextArg(args, i++, "namespace") ?? namespaceName;
                break;
            case "-c"
            or "--class-name":
                className = GetNextArg(args, i++, "class-name") ?? className;
                break;
            case "-b"
            or "--base-url":
                baseUrl = GetNextArg(args, i++, "base-url");
                break;
            case "-v"
            or "--version":
                version = GetNextArg(args, i++, "version");
                break;
            default:
                break;
        }
    }

    if (string.IsNullOrEmpty(openApiUrl))
    {
        Console.WriteLine("Error: --openapi-url is required");
        PrintUsage();
        return null;
    }

    if (string.IsNullOrEmpty(outputPath))
    {
        Console.WriteLine("Error: --output-path is required");
        PrintUsage();
        return null;
    }

    return new Config(openApiUrl, outputPath, namespaceName, className, baseUrl, version);
}

static string? GetNextArg(string[] args, int currentIndex, string optionName)
{
    if (currentIndex + 1 >= args.Length)
    {
        Console.WriteLine($"Error: --{optionName} requires a value");
        return null;
    }

    return args[currentIndex + 1];
}

static async Task GenerateCode(Config config)
{
    Console.WriteLine("RestClient.Net OpenAPI Code Generator");
    Console.WriteLine("=====================================\n");

    string openApiSpec;

    var isUrl =
        config.OpenApiUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        || config.OpenApiUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    if (!isUrl)
    {
        var filePath = config.OpenApiUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase)
            ? config.OpenApiUrl[7..]
            : config.OpenApiUrl;

        Console.WriteLine($"Reading OpenAPI spec from file: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found: {filePath}");
            return;
        }

        openApiSpec = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
    }
    else
    {
        var services = new ServiceCollection();
        _ = services.AddHttpClient();
        using var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        Console.WriteLine($"Downloading OpenAPI spec from: {config.OpenApiUrl}");
        using var httpClient = httpClientFactory.CreateClient();
        var downloadResult = await httpClient
            .GetAsync(
                new Urls.AbsoluteUrl(config.OpenApiUrl),
                deserializeSuccess: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                deserializeError: async (response, ct) =>
                    await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false),
                cancellationToken: CancellationToken.None
            )
            .ConfigureAwait(false);

#pragma warning disable CS8509
        openApiSpec = downloadResult switch
        {
            OkString(var success) => success,
            ErrorString(ExceptionErrorString(var ex)) => HandleDownloadError(
                $"Exception: {ex.Message}"
            ),
            ErrorString(ResponseErrorString(var body, var statusCode, _)) => HandleDownloadError(
                $"HTTP {statusCode}: {body}"
            ),
        };
#pragma warning restore CS8509

        if (string.IsNullOrEmpty(openApiSpec))
        {
            return;
        }
    }

    Console.WriteLine($"Downloaded {openApiSpec.Length} characters\n");
    Console.WriteLine("Generating C# code from OpenAPI spec...");

    var generatorResult = OpenApiCodeGenerator.Generate(
        openApiSpec,
        @namespace: config.Namespace,
        className: config.ClassName,
        outputPath: config.OutputPath,
        baseUrlOverride: config.BaseUrl,
        versionOverride: config.Version
    );

    Console.WriteLine(
        $"Generated {generatorResult.ExtensionMethodsCode.Length} "
            + "characters of extension methods"
    );
    Console.WriteLine($"Generated {generatorResult.ModelsCode.Length} " + "characters of models");

    if (generatorResult.ExtensionMethodsCode.StartsWith("//", StringComparison.Ordinal))
    {
        Console.WriteLine("\nError in generated code:");
        Console.WriteLine(generatorResult.ExtensionMethodsCode);
        return;
    }

    Console.WriteLine($"\nSaved files to: {config.OutputPath}");
    Console.WriteLine("\nGeneration completed successfully!");
}

static string HandleDownloadError(string message)
{
    Console.WriteLine($"Failed to download OpenAPI spec - {message}");
    return string.Empty;
}

internal sealed record Config(
    string OpenApiUrl,
    string OutputPath,
    string Namespace,
    string ClassName,
    string? BaseUrl,
    string? Version
);
