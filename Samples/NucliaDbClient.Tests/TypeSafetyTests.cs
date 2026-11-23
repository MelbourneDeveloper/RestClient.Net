using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Generated;
using Outcome;
using Xunit;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line

namespace NucliaDbClient.Tests;

/// <summary>
/// Tests to verify that issue #142 is fixed: anyOf types are now properly resolved
/// to their concrete types instead of being generated as 'object'.
/// </summary>
[Collection("NucliaDB Tests")]
[TestCaseOrderer("NucliaDbClient.Tests.PriorityOrderer", "NucliaDbClient.Tests")]
public class TypeSafetyTests
{
    #region Setup
    private readonly IHttpClientFactory _httpClientFactory;
    private static string? _knowledgeBoxId;

    public TypeSafetyTests(NucliaDbFixture fixture)
    {
        _ = fixture; // Ensure fixture is initialized
        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    private HttpClient CreateHttpClient() => _httpClientFactory.CreateClient();
    #endregion

    [Fact]
    [TestPriority(-1)]
    public async Task CreateKnowledgeBox_ForTypeTests()
    {
        var payload = new { slug = $"typesafe-kb-{Guid.NewGuid()}", title = "Type Safety Test KB" };

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

    /// <summary>
    /// Verifies that KnowledgeBoxObj.Config is typed as KnowledgeBoxConfig (not object).
    /// This is a compile-time check - if Config were 'object', the property access would fail.
    /// </summary>
    /// <returns>Task.</returns>
    [Fact]
    [TestPriority(1)]
    public async Task KnowledgeBoxObj_Config_IsTypedAsKnowledgeBoxConfig_NotObject()
    {
        // Arrange & Act
        var result = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

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

        // Assert - These assertions will only compile if Config is KnowledgeBoxConfig (not object)
        Assert.NotNull(kb);

        // If Config is object, we can't access these properties without casting
        // The fact this compiles proves Config is KnowledgeBoxConfig
        if (kb.Config is { } config)
        {
            // Access Config properties in a type-safe manner
            var slug = config.Slug;
            var hiddenResourcesEnabled = config.HiddenResourcesEnabled;
            var hiddenResourcesHideOnCreation = config.HiddenResourcesHideOnCreation;

            // These properties exist on KnowledgeBoxConfig but not on object
            Assert.NotNull(slug);

            // Verify we can access the bool properties (wouldn't work if Config were object)
            // The fact that this compiles without casting proves the types are correct
            _ = hiddenResourcesEnabled;
            _ = hiddenResourcesHideOnCreation;
        }
    }

    /// <summary>
    /// Verifies that KnowledgeBoxObj.Model is typed as SemanticModelMetadata (not object).
    /// </summary>
    /// <returns>Task.</returns>
    [Fact]
    [TestPriority(2)]
    public async Task KnowledgeBoxObj_Model_IsTypedAsSemanticModelMetadata_NotObject()
    {
        // Arrange & Act
        var result = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

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

        // Assert - These assertions will only compile if Model is SemanticModelMetadata (not object)
        Assert.NotNull(kb);

        if (kb.Model is { } model)
        {
            // Access Model properties in a type-safe manner
            var similarityFunction = model.SimilarityFunction;
            var vectorDimension = model.VectorDimension;

            // These properties exist on SemanticModelMetadata but not on object
            Assert.NotNull(similarityFunction);
            Assert.NotEmpty(similarityFunction);

            // Vector dimension should be a positive integer if present
            if (vectorDimension.HasValue)
            {
                Assert.True(vectorDimension.Value > 0);
            }
        }
    }

    /// <summary>
    /// Verifies we can access deeply nested properties in a type-safe manner.
    /// This proves the entire type hierarchy is properly resolved.
    /// </summary>
    /// <returns>Task.</returns>
    [Fact]
    [TestPriority(3)]
    public async Task KnowledgeBoxObj_AllowsDeepPropertyAccess_TypeSafely()
    {
        // Arrange & Act
        var result = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

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

        // Assert - Access multiple levels of properties without casting
        Assert.NotNull(kb);
        Assert.NotNull(kb.Uuid);
        Assert.NotNull(kb.Slug);

        // Access Config properties - the fact that we can do this without casting proves type safety
        var configSlug = kb.Config?.Slug;
        var configTitle = kb.Config?.Title;

        // The fact that we can access these properties without casting proves type safety
        // If Config/Model were 'object', we'd need explicit casts
        Assert.NotNull(configSlug);
        Assert.NotNull(configTitle);
    }

    /// <summary>
    /// Verifies that we can use pattern matching with the properly typed properties.
    /// </summary>
    /// <returns>Task.</returns>
    [Fact]
    [TestPriority(4)]
    public async Task KnowledgeBoxObj_SupportsPatternMatching_OnTypedProperties()
    {
        // Arrange & Act
        var result = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

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

        // Assert - Use pattern matching on typed properties
        Assert.NotNull(kb);

        // Pattern match on Config (only works if Config is KnowledgeBoxConfig, not object)
        var configDescription = kb.Config switch
        {
            { Description: var desc } when !string.IsNullOrEmpty(desc) => desc,
            { Description: _ } => "No description",
            null => "Config not available",
        };

        Assert.NotNull(configDescription);

        // Pattern match on Model (only works if Model is SemanticModelMetadata, not object)
        var modelDescription = kb.Model switch
        {
            { SimilarityFunction: var sf, VectorDimension: var vd }
                when !string.IsNullOrEmpty(sf) => $"{sf} with dimension {vd}",
            { SimilarityFunction: var sf } => sf,
            null => "Model not available",
        };

        Assert.NotNull(modelDescription);
    }

    /// <summary>
    /// Verifies that type information is preserved across multiple API calls.
    /// </summary>
    /// <returns>Task.</returns>
    [Fact]
    [TestPriority(5)]
    public async Task MultipleApiCalls_ReturnConsistentlyTypedObjects()
    {
        // First call
        var result1 = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

        var kb1 = result1 switch
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

        // Second call
        var result2 = await CreateHttpClient()
            .KbKbKbidGetAsync(kbid: _knowledgeBoxId!, xNUCLIADBROLES: "READER");

        var kb2 = result2 switch
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

        // Assert - Both objects have properly typed Config and Model
        Assert.NotNull(kb1);
        Assert.NotNull(kb2);

        // Access Config from both - proves type consistency
        var config1Slug = kb1.Config?.Slug;
        var config2Slug = kb2.Config?.Slug;

        Assert.Equal(config1Slug, config2Slug);

        // Access Model from both - proves type consistency
        var model1Func = kb1.Model?.SimilarityFunction;
        var model2Func = kb2.Model?.SimilarityFunction;

        Assert.Equal(model1Func, model2Func);
    }

    /// <summary>
    /// Compile-time type verification test.
    /// This method contains code that will ONLY compile if the types are correct.
    /// If Config or Model were 'object', this would not compile without casts.
    /// </summary>
    [Fact]
    [TestPriority(6)]
    public void CompileTimeTypeVerification_ProvesBugIsFix()
    {
        // This is a compile-time test - if it compiles, the types are correct

        // Create a mock KnowledgeBoxObj
        var config = new KnowledgeBoxConfig(
            Slug: "test",
            Title: "Test",
            Description: "Test",
            LearningConfiguration: null,
            ExternalIndexProvider: null,
            ConfiguredExternalIndexProvider: null,
            Similarity: null,
            HiddenResourcesEnabled: false,
            HiddenResourcesHideOnCreation: false
        );

        var model = new SemanticModelMetadata(
            SimilarityFunction: "cosine",
            VectorDimension: 768,
            DefaultMinScore: 0.7f
        );

        var kb = new KnowledgeBoxObj(
            Slug: "test-kb",
            Uuid: Guid.NewGuid().ToString(),
            Config: config,
            Model: model
        );

        // These assignments will ONLY compile if the types are correct
        var configFromKb = kb.Config; // Would fail if Config were object
        var modelFromKb = kb.Model; // Would fail if Model were object

        // Access properties without casting
        var slug = configFromKb?.Slug;
        var similarityFunction = modelFromKb?.SimilarityFunction;
        var vectorDimension = modelFromKb?.VectorDimension;

        // Assert
        Assert.NotNull(configFromKb);
        Assert.NotNull(modelFromKb);
        Assert.Equal("test", slug);
        Assert.Equal("cosine", similarityFunction);
        Assert.Equal(768, vectorDimension);
    }
}
