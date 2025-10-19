using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using NucliaDB.Mcp;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging to stderr to not interfere with stdio MCP protocol
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Get the NucliaDB base URL from environment or use default
var nucleaBaseUrl =
    Environment.GetEnvironmentVariable("NUCLIA_BASE_URL") ?? "http://localhost:8080/api/v1";

// Configure HttpClient with base URL
builder.Services.AddHttpClient(
    string.Empty,
    client =>
    {
        client.BaseAddress = new Uri(nucleaBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    }
);

// Add the NucliaDB tools to DI
builder.Services.AddSingleton<NucliaDbTools>();

// Add MCP server with stdio transport and tools from assembly
builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();

var host = builder.Build();
await host.RunAsync();
