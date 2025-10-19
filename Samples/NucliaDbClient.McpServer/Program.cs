using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;

var builder = Host.CreateApplicationBuilder(args);

// Get the NucliaDB base URL from environment or use default
var nucleaBaseUrl =
    Environment.GetEnvironmentVariable("NUCLIA_BASE_URL") ?? "http://localhost:8080/api/v1";

// Configure HttpClient with base URL
builder.Services.AddHttpClient(
    Options.DefaultName,
    client =>
    {
        client.BaseAddress = new Uri(nucleaBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    }
);

// Add MCP server with NucliaDB tools
builder.Services
    .AddMcpServer(new ServerInfo(name: "nuclia-db-mcp-server", version: "1.0.0"))
    .WithToolsFromAssembly();

var host = builder.Build();
await host.RunAsync();
