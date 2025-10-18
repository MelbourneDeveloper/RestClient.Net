using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Generated;
using Outcome;
using Xunit;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

namespace NucliaDbClient.Tests;

public class NucliaDbApiTests
{
    private readonly IHttpClientFactory _httpClientFactory;

    public NucliaDbApiTests()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    [Fact]
    public async Task GetKnowledgeBox_ReturnsValidData()
    {
        // Act
        var result = await _httpClientFactory
            .CreateClient()
            .GetKbKbKbidGet("2edd5a30-8e28-4185-a0be-629971a9784c", "READER")
            .ConfigureAwait(false);

        // Assert
        var kb = result switch
        {
            OkKnowledgeBoxObj(var value) => value,
            ErrorKnowledgeBoxObj(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
        };

        Assert.NotNull(kb);
        Assert.NotNull(kb.Slug);
        Assert.NotNull(kb.Uuid);
    }

    [Fact]
    public async Task ListResources_ReturnsResourceList()
    {
        // Act
        var result = await _httpClientFactory
            .CreateClient()
            .ListResourcesKbKbidResourcesGet(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                0,
                10,
                "READER"
            )
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
