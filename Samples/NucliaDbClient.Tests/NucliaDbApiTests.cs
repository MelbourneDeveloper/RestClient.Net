using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Generated;
using Outcome;
using Xunit;

namespace NucliaDbClient.Tests;

public class NucliaDbApiTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string? _kbid;

    public NucliaDbApiTests()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        _kbid = Environment.GetEnvironmentVariable("NUCLIA_KBID");
    }

    [SkippableFact]
    public async Task GetKnowledgeBox_ReturnsValidData()
    {
        // Arrange
        var httpClient = _httpClientFactory.CreateClient();

        // Act
        var result = await httpClient.GetKbKbKbidGet(_kbid!).ConfigureAwait(false);

        // Assert
        var kb = result switch
        {
            OkKnowledgeBoxObj(var value) => value,
            ErrorKnowledgeBoxObj(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorKnowledgeBoxObj(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
            _ => throw new InvalidOperationException("Unexpected result type"),
        };

        Assert.NotNull(kb);
        Assert.NotNull(kb.Slug);
        Assert.NotNull(kb.Uuid);
    }

    [SkippableFact]
    public async Task ListResources_ReturnsResourceList()
    {
        // Arrange
        var httpClient = _httpClientFactory.CreateClient();

        // Act
        var result = await httpClient
            .ListResourcesKbKbidResourcesGet(_kbid!, 0, 10)
            .ConfigureAwait(false);

        // Assert
        var resources = result switch
        {
            OkResourceList(var value) => value,
            ErrorResourceList(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceList(HttpError<string>.ErrorResponseError(var body, var statusCode, _)) =>
                throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
            _ => throw new InvalidOperationException("Unexpected result type"),
        };

        Assert.NotNull(resources);
    }
}
