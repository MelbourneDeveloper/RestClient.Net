using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Generated;
using Outcome;
using Xunit;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line

namespace NucliaDbClient.Tests;

[Collection("NucliaDB Tests")]
[TestCaseOrderer("NucliaDbClient.Tests.PriorityOrderer", "NucliaDbClient.Tests")]
public class NucliaDbApiTests
{
    #region Setup
    private readonly IHttpClientFactory _httpClientFactory;
    private static string? _createdResourceId;
    private static string? _knowledgeBoxId;

    public NucliaDbApiTests(NucliaDbFixture fixture)
    {
        _ = fixture; // Ensure fixture is initialized
        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    private HttpClient CreateHttpClient() => _httpClientFactory.CreateClient();

    private async Task<string> EnsureResourceExists()
    {
        if (!string.IsNullOrEmpty(_createdResourceId))
        {
            return _createdResourceId;
        }

        var payload = new CreateResourcePayload(
            Title: "Test Resource",
            Summary: null,
            Slug: $"test-resource-{Guid.NewGuid()}",
            Icon: null,
            Thumbnail: null,
            Metadata: null,
            Usermetadata: null,
            Fieldmetadata: null,
            Origin: null,
            Extra: null,
            Hidden: null,
            Files: new Dictionary<string, FileField>(),
            Links: new Dictionary<string, LinkField>(),
            Texts: new Dictionary<string, TextField>(),
            Conversations: new Dictionary<string, InputConversationField>(),
            ProcessingOptions: null,
            Security: null
        );

        var result = await CreateHttpClient()
            .CreateResourceKbKbidResourcesAsync(
                kbid: _knowledgeBoxId!,
                body: payload,
                xSkipStore: false,
                xNucliadbUser: "test-user",
                xNUCLIADBROLES: "WRITER"
            );

        var created = result switch
        {
            OkResourceCreatedHTTPValidationError(var value) => value,
            ErrorResourceCreatedHTTPValidationError(
                HttpError<HTTPValidationError>.ExceptionError
                (var ex)
            ) => throw new InvalidOperationException("Failed to create resource", ex),
            ErrorResourceCreatedHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException(
                $"Failed to create resource: HTTP {statusCode}: {body}"
            ),
        };

        _createdResourceId = created.Uuid!;
        return _createdResourceId;
    }

    #endregion

    #region Tests

    [Fact]
    [TestPriority(-1)]
    public async Task CreateKnowledgeBox_CreatesKB()
    {
        var payload = new { slug = $"test-kb-{Guid.NewGuid()}", title = "Test Knowledge Box" };

        var result = await CreateHttpClient()
            .CreateKnowledgeBoxKbsAsync(body: payload, xNUCLIADBROLES: "MANAGER");

        var kb = result switch
        {
            OkKnowledgeBoxObj(var value) => value,
            ErrorKnowledgeBoxObj(HttpError<string>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("Failed to create knowledge box", ex),
            ErrorKnowledgeBoxObj(
                HttpError<string>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException(
                $"Failed to create knowledge box: HTTP {statusCode}: {body}"
            ),
        };

        Assert.NotNull(kb);
        Assert.NotNull(kb.Uuid);

        _knowledgeBoxId = kb.Uuid;
    }

    [Fact]
    [TestPriority(0)]
    public async Task CreateResource_ReturnsResourceCreated_WithoutAuthentication()
    {
        var payload = new CreateResourcePayload(
            Title: "Test Resource",
            Summary: null,
            Slug: $"test-resource-{Guid.NewGuid()}",
            Icon: null,
            Thumbnail: null,
            Metadata: null,
            Usermetadata: null,
            Fieldmetadata: null,
            Origin: null,
            Extra: null,
            Hidden: null,
            Files: new Dictionary<string, FileField>(),
            Links: new Dictionary<string, LinkField>(),
            Texts: new Dictionary<string, TextField>(),
            Conversations: new Dictionary<string, InputConversationField>(),
            ProcessingOptions: null,
            Security: null
        );

        var result = await CreateHttpClient()
            .CreateResourceKbKbidResourcesAsync(
                kbid: _knowledgeBoxId!,
                body: payload,
                xSkipStore: false,
                xNucliadbUser: "test-user",
                xNUCLIADBROLES: "WRITER"
            );

        var created = result switch
        {
            OkResourceCreatedHTTPValidationError(var value) => value,
            ErrorResourceCreatedHTTPValidationError(
                HttpError<HTTPValidationError>.ExceptionError
                (var ex)
            ) => throw new InvalidOperationException("Failed to create resource", ex),
            ErrorResourceCreatedHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException(
                $"Failed to create resource: HTTP {statusCode}: {body}"
            ),
        };

        Assert.NotNull(created);
        Assert.NotNull(created.Uuid);
        Assert.NotNull(created.Seqid);
    }

    [Fact]
    [TestPriority(1)]
    public async Task GetKnowledgeBox_ReturnsValidData()
    {
        // Act
        var result = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

        // Assert
        var kb = result switch
        {
            OkKnowledgeBoxObjHTTPValidationError(var value) => value,
            ErrorKnowledgeBoxObjHTTPValidationError(
                HttpError<HTTPValidationError>.ExceptionError
                (var ex)
            ) => throw new InvalidOperationException("API call failed with exception", ex),
            ErrorKnowledgeBoxObjHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(kb);
        Assert.NotNull(kb.Slug);
        Assert.NotNull(kb.Uuid);
        Assert.Equal(_knowledgeBoxId, kb.Uuid);
    }

    [Fact]
    [TestPriority(3)]
    public async Task ListResources_ReturnsResourceList()
    {
        // Ensure there's at least one resource in the KB (for fresh container robustness)
        await EnsureResourceExists();

        // Act
        var result = await CreateHttpClient()
            .ListResourcesKbKbidResourcesAsync(
                kbid: _knowledgeBoxId!,
                page: 0,
                size: 10,
                xNUCLIADBROLES: "READER"
            );

        // Assert
        var resources = result switch
        {
            OkResourceListHTTPValidationError(var value) => value,
            ErrorResourceListHTTPValidationError(
                HttpError<HTTPValidationError>.ExceptionError
                (var ex)
            ) => throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceListHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(resources);
        Assert.NotNull(resources.Resources);
        Assert.NotEmpty(resources.Resources);
    }

    [Fact]
    [TestPriority(4)]
    public async Task GetResource_ReturnsResource()
    {
        // Ensure resource exists
        var resourceId = await EnsureResourceExists();

        // Act
        var result = await CreateHttpClient()
            .ResourceByUuidKbKbidResourceRidGetAsync(
                kbid: _knowledgeBoxId!,
                rid: resourceId,
                show: [],
                fieldType: [],
                extracted: [],
                xNucliadbUser: "test-user",
                xForwardedFor: "127.0.0.1",
                xNUCLIADBROLES: "READER"
            );

        // Assert
        var resource = result switch
        {
            OkNucliadbModelsResourceResourceHTTPValidationError(var value) => value,
            ErrorNucliadbModelsResourceResourceHTTPValidationError(HttpError<HTTPValidationError>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorNucliadbModelsResourceResourceHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
                (var body, var statusCode, _)
            ) => throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.NotNull(resource);
        Assert.NotNull(resource.Id);
    }

    [Fact]
    [TestPriority(5)]
    public async Task ModifyResource_ReturnsResourceUpdated()
    {
        // Ensure resource exists
        var resourceId = await EnsureResourceExists();

        // Act
        var updatePayload = new UpdateResourcePayload(
            Title: "Updated Title",
            Summary: null,
            Slug: null,
            Thumbnail: null,
            Metadata: null,
            Usermetadata: null,
            Fieldmetadata: null,
            Origin: null,
            Extra: null,
            Files: new Dictionary<string, FileField>(),
            Links: new Dictionary<string, LinkField>(),
            Texts: new Dictionary<string, TextField>(),
            Conversations: new Dictionary<string, InputConversationField>(),
            ProcessingOptions: null,
            Security: null,
            Hidden: null
        );

        var result = await CreateHttpClient()
            .ModifyResourceRidPrefixKbKbidResourceRidAsync(
                kbid: _knowledgeBoxId!,
                rid: resourceId,
                body: updatePayload,
                xNucliadbUser: "test-user",
                xSkipStore: false,
                xNUCLIADBROLES: "WRITER"
            );

        // Assert
        var updated = result switch
        {
            OkResourceUpdatedHTTPValidationError(var value) => value,
            ErrorResourceUpdatedHTTPValidationError(HttpError<HTTPValidationError>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceUpdatedHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
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
            .KnowledgeboxCountersKbKbidCountersAsync(
                kbid: _knowledgeBoxId!,
                debug: false,
                xNUCLIADBROLES: "READER"
            );

        // Assert
        var counters = result switch
        {
            OkKnowledgeboxCountersHTTPValidationError(var value) => value,
            ErrorKnowledgeboxCountersHTTPValidationError(HttpError<HTTPValidationError>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorKnowledgeboxCountersHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
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
        // Ensure resource exists
        var resourceId = await EnsureResourceExists();

        // Act
        var textField = new TextField(
            Body: "This is test text content",
            Format: "PLAIN",
            ExtractStrategy: null,
            SplitStrategy: null
        );
        var result = await CreateHttpClient()
            .AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdAsync(
                kbid: _knowledgeBoxId!,
                rid: resourceId,
                fieldId: "test-field",
                body: textField,
                xNUCLIADBROLES: "WRITER"
            );

        // Assert
        var fieldAdded = result switch
        {
            OkResourceFieldAddedHTTPValidationError(var value) => value,
            ErrorResourceFieldAddedHTTPValidationError(HttpError<HTTPValidationError>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorResourceFieldAddedHTTPValidationError(
                HttpError<HTTPValidationError>.ErrorResponseError
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
        // Ensure resource exists
        var resourceId = await EnsureResourceExists();

        // Act
        var result = await CreateHttpClient()
            .ResourceRidPrefixKbKbidResourceRidDeleteAsync(
                kbid: _knowledgeBoxId!,
                rid: resourceId,
                xNUCLIADBROLES: "WRITER"
            );

        // Assert
        var unit = result switch
        {
            OkUnitHTTPValidationError(var value) => value,
            ErrorUnitHTTPValidationError(HttpError<HTTPValidationError>.ExceptionError(var ex)) =>
                throw new InvalidOperationException("API call failed with exception", ex),
            ErrorUnitHTTPValidationError(HttpError<HTTPValidationError>.ErrorResponseError(var body, var statusCode, _)) =>
                throw new InvalidOperationException($"API call failed: HTTP {statusCode}: {body}"),
        };

        Assert.Equal(Unit.Value, unit);

        // Clear the resource ID since it's been deleted
        _createdResourceId = null;
    }
    #endregion
}
