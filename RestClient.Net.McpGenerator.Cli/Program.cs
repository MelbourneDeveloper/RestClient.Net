#pragma warning disable CA1502

using RestClient.Net.McpGenerator;

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
    Console.WriteLine("RestClient.Net MCP Server Generator");
    Console.WriteLine("====================================\n");
    Console.WriteLine("Generates MCP tool code that wraps RestClient.Net extension methods.\n");
    Console.WriteLine("Usage:");
    Console.WriteLine("  mcp-generator [options]\n");
    Console.WriteLine("Options:");
    Console.WriteLine(
        "  -u, --openapi-url <url>       (Required) URL or file path to OpenAPI spec"
    );
    Console.WriteLine(
        "  -o, --output-file <path>      (Required) Output file path for generated code"
    );
    Console.WriteLine(
        "  -n, --namespace <namespace>   MCP server namespace (default: 'McpServer')"
    );
    Console.WriteLine("  -s, --server-name <name>      MCP server name (default: 'ApiMcp')");
    Console.WriteLine(
        "  --ext-namespace <namespace>   Extensions namespace (default: 'Generated')"
    );
    Console.WriteLine(
        "  --ext-class <class>           Extensions class name (default: 'ApiExtensions')"
    );
    Console.WriteLine("  -h, --help                    Show this help message");
}

static Config? ParseArgs(string[] args)
{
    string? openApiUrl = null;
    string? outputFile = null;
    var namespaceName = "McpServer";
    var serverName = "ApiMcp";
    var extensionsNamespace = "Generated";
    var extensionsClass = "ApiExtensions";

    for (var i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "-u"
            or "--openapi-url":
                openApiUrl = GetNextArg(args, i++, "openapi-url");
                break;
            case "-o"
            or "--output-file":
                outputFile = GetNextArg(args, i++, "output-file");
                break;
            case "-n"
            or "--namespace":
                namespaceName = GetNextArg(args, i++, "namespace") ?? namespaceName;
                break;
            case "-s"
            or "--server-name":
                serverName = GetNextArg(args, i++, "server-name") ?? serverName;
                break;
            case "--ext-namespace":
                extensionsNamespace = GetNextArg(args, i++, "ext-namespace") ?? extensionsNamespace;
                break;
            case "--ext-class":
                extensionsClass = GetNextArg(args, i++, "ext-class") ?? extensionsClass;
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

    if (string.IsNullOrEmpty(outputFile))
    {
        Console.WriteLine("Error: --output-file is required");
        PrintUsage();
        return null;
    }

    return new Config(
        openApiUrl,
        outputFile,
        namespaceName,
        serverName,
        extensionsNamespace,
        extensionsClass
    );
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
    Console.WriteLine("RestClient.Net MCP Server Generator");
    Console.WriteLine("====================================\n");

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
        Console.WriteLine($"Downloading OpenAPI spec from: {config.OpenApiUrl}");
        using var httpClient = new HttpClient();
        openApiSpec = await httpClient.GetStringAsync(config.OpenApiUrl).ConfigureAwait(false);
    }

    Console.WriteLine($"Read {openApiSpec.Length} characters\n");
    Console.WriteLine("Generating MCP tools code...");

    var result = McpServerGenerator.Generate(
        openApiSpec,
        @namespace: config.Namespace,
        serverName: config.ServerName,
        extensionsNamespace: config.ExtensionsNamespace
    );

#pragma warning disable IDE0010
    switch (result)
#pragma warning restore IDE0010
    {
        case Outcome.Result<string, string>.Ok<string, string>(var code):
            await File.WriteAllTextAsync(config.OutputFile, code).ConfigureAwait(false);
            Console.WriteLine($"Generated {code.Length} characters of MCP tools code");
            Console.WriteLine($"\nSaved to: {config.OutputFile}");
            Console.WriteLine("\nGeneration completed successfully!");
            break;
        case Outcome.Result<string, string>.Error<string, string>(var error):
            Console.WriteLine("\nCode generation failed:");
            Console.WriteLine(error);
            break;
    }
}

internal sealed record Config(
    string OpenApiUrl,
    string OutputFile,
    string Namespace,
    string ServerName,
    string ExtensionsNamespace,
    string ExtensionsClass
);
