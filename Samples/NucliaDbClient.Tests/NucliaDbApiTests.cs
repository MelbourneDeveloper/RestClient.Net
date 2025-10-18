using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Generated;
using Outcome;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead

namespace NucliaDbClient.Tests;

[Collection("NucliaDB Tests")]
[TestCaseOrderer("NucliaDbClient.Tests.PriorityOrderer", "NucliaDbClient.Tests")]
public class NucliaDbApiTests
{
    private readonly IHttpClientFactory _httpClientFactory;

    // DO NOT PUT VARIABLES HERE. NO SHARED STATE BETWEEN TESTS.
    private static string? _createdResourceId;

    public NucliaDbApiTests()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    private HttpClient CreateHttpClient() => _httpClientFactory.CreateClient();

    [Fact]
    [TestPriority(1)]
    public async Task GetKnowledgeBox_ReturnsValidData()
    {
        // Act
        var result = await CreateHttpClient()
            .GetKbKbKbidGet("2edd5a30-8e28-4185-a0be-629971a9784c", "READER")
            .ConfigureAwait(false);

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
        };

        Assert.NotNull(kb);
        Assert.NotNull(kb.Slug);
        Assert.NotNull(kb.Uuid);
        Assert.Equal("2edd5a30-8e28-4185-a0be-629971a9784c", kb.Uuid);
    }

    [Fact]
    [TestPriority(2)]
    public async Task CreateResource_ReturnsResourceCreated()
    {
        // Act
        var payload = new CreateResourcePayload
        {
            Slug = $"test-resource-{Guid.NewGuid()}",
            Title = "Test Resource",
            Texts = new Dictionary<string, TextField>(),
            Files = new Dictionary<string, FileField>(),
            Links = new Dictionary<string, LinkField>(),
            Conversations = new Dictionary<string, InputConversationField>(),
        };

        var result = await CreateHttpClient()
            .CreateResourceKbKbidResourcesPost(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                false,
                "test-user",
                "WRITER",
                payload
            )
            .ConfigureAwait(false);

        // Assert
        var created = result switch
        {
            OkResourceCreated(var value) => value,
            ErrorResourceCreated(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceCreated(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(created);
        Assert.NotNull(created.Uuid);
        Assert.NotEmpty(created.Uuid);
        _createdResourceId = created.Uuid;
    }

    [Fact]
    [TestPriority(3)]
    public async Task ListResources_ReturnsResourceList()
    {
        // Act
        var result = await CreateHttpClient()
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
        };

        Assert.NotNull(resources);
        Assert.NotNull(resources.Resources);
        Assert.NotEmpty(resources.Resources);
    }

    [Fact]
    [TestPriority(4)]
    public async Task GetResource_ReturnsResource()
    {
        if (_createdResourceId == null)
        {
            return;
        }

        // Act
        var result = await CreateHttpClient()
            .GetResourceByUuidKbKbidResourceRidGet(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                _createdResourceId!,
                [],
                [],
                [],
                "test-user",
                "127.0.0.1",
                "READER"
            )
            .ConfigureAwait(false);

        // Assert
        var resource = result switch
        {
            OkNucliadbModelsResourceResource(var value) => value,
            ErrorNucliadbModelsResourceResource(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorNucliadbModelsResourceResource(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(resource);
        Assert.NotNull(resource.Id);
        Assert.Equal(_createdResourceId, resource.Id);
    }

    [Fact]
    [TestPriority(5)]
    public async Task ModifyResource_ReturnsResourceUpdated()
    {
        if (_createdResourceId == null)
        {
            return;
        }

        // Act
        var updatePayload = new UpdateResourcePayload
        {
            Title = "Updated Title",
            Texts = new Dictionary<string, TextField>(),
            Files = new Dictionary<string, FileField>(),
            Links = new Dictionary<string, LinkField>(),
            Conversations = new Dictionary<string, InputConversationField>(),
        };

        var result = await CreateHttpClient()
            .ModifyResourceRidPrefixKbKbidResourceRidPatch(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                _createdResourceId!,
                "test-user",
                false,
                "WRITER",
                updatePayload
            )
            .ConfigureAwait(false);

        // Assert
        var updated = result switch
        {
            OkResourceUpdated(var value) => value,
            ErrorResourceUpdated(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceUpdated(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(updated);
        Assert.NotNull(updated.Seqid);
    }

    [Fact]
    [TestPriority(6)]
    public async Task GetKnowledgeBoxCounters_ReturnsCounters()
    {
        // Act
        var result = await CreateHttpClient()
            .KnowledgeboxCountersKbKbidCountersGet(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                false,
                "READER"
            )
            .ConfigureAwait(false);

        // Assert
        var counters = result switch
        {
            OkKnowledgeboxCounters(var value) => value,
            ErrorKnowledgeboxCounters(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorKnowledgeboxCounters(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(counters);
        Assert.True(counters.Resources >= 1, "Should have at least 1 resource");
    }

    [Fact]
    [TestPriority(9)]
    public async Task AddTextFieldToResource_ReturnsResourceFieldAdded()
    {
        if (_createdResourceId == null)
        {
            return;
        }

        // Act
        var textField = new TextField { Body = "This is test text content", Format = "PLAIN" };
        var result = await CreateHttpClient()
            .AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                _createdResourceId!,
                "test-field",
                "WRITER",
                textField
            )
            .ConfigureAwait(false);

        // Assert
        var fieldAdded = result switch
        {
            OkResourceFieldAdded(var value) => value,
            ErrorResourceFieldAdded(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceFieldAdded(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(fieldAdded);
        Assert.NotNull(fieldAdded.Seqid);
    }

    [Fact]
    [TestPriority(10)]
    public async Task DeleteResource_ReturnsUnit()
    {
        if (_createdResourceId == null)
        {
            return;
        }

        // Act
        var result = await CreateHttpClient()
            .DeleteResourceRidPrefixKbKbidResourceRidDelete(
                "2edd5a30-8e28-4185-a0be-629971a9784c",
                _createdResourceId!,
                "WRITER"
            )
            .ConfigureAwait(false);

        // Assert
        var unit = result switch
        {
            OkUnit(var value) => value,
            ErrorUnit(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorUnit(HttpError<string>.ErrorResponseError(var body, var statusCode, _)) =>
                throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.Equal(Unit.Value, unit);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class TestPriorityAttribute : Attribute
{
    public int Priority { get; }

    public TestPriorityAttribute(int priority) => Priority = priority;
}

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase =>
        testCases.OrderBy(tc =>
            tc.TestMethod.Method.GetCustomAttributes(
                    typeof(TestPriorityAttribute).AssemblyQualifiedName!
                )
                .FirstOrDefault()
                ?.GetNamedArgument<int>("Priority") ?? 0
        );
}
