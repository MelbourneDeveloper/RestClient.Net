using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Mcp;

// Get the NucliaDB base URL from environment or use default
var nucleaBaseUrl =
    Environment.GetEnvironmentVariable("NUCLIA_BASE_URL") ?? "http://localhost:8080/api/v1";

// Create a simple HTTP client factory
var services = new ServiceCollection();

// Configure HttpClient with base URL
services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri(nucleaBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add the NucliaDB tools to DI
services.AddSingleton<NucliaDbTools>();

var serviceProvider = services.BuildServiceProvider();

// TODO: Wire up MCP server when ModelContextProtocol API stabilizes
Console.WriteLine("NucliaDB MCP Server - MCP tools generated successfully!");
Console.WriteLine($"Configured for NucliaDB at: {nucleaBaseUrl}");
Console.WriteLine("Ready to integrate with ModelContextProtocol when API is stable.");

await Task.CompletedTask.ConfigureAwait(false);
