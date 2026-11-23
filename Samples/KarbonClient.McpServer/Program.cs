using Karbon.Mcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging to stderr to not interfere with stdio MCP protocol
builder.Logging.AddConsole(consoleLogOptions =>
{
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Get the Karbon base URL from environment or use default
var karbonBaseUrl =
    Environment.GetEnvironmentVariable("KARBON_BASE_URL") ?? "https://api.karbonhq.com";

// Get Karbon credentials from environment
var authorizationToken = Environment.GetEnvironmentVariable("KARBON_AUTHORIZATION_TOKEN");
var accessKey = Environment.GetEnvironmentVariable("KARBON_ACCESS_KEY");

// Configure default HttpClient with base URL and authentication headers
builder.Services.AddHttpClient(
    string.Empty,
    client =>
    {
        client.BaseAddress = new Uri(karbonBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);

        if (!string.IsNullOrEmpty(authorizationToken))
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authorizationToken}");
        }

        if (!string.IsNullOrEmpty(accessKey))
        {
            client.DefaultRequestHeaders.Add("AccessKey", accessKey);
        }
    }
);

// Add MCP server with stdio transport and tools from assembly
builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();

var host = builder.Build();
await host.RunAsync();
