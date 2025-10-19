#nullable enable
using System.ComponentModel;
using System.Text.Json;
using Outcome;
using NucliaDB.Generated;

namespace NucliaDB.Mcp;

/// <summary>MCP server tools for NucliaDb API.</summary>
public class NucliaDbTools(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="slug">slug</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> KbBySlugKbSSlugGet(string slug)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KbBySlugKbSSlugGetAsync(slug, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxObjHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxObjHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> KbKbKbidGet(string kbid, string xNUCLIADBROLES = "READER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KbKbKbidGetAsync(kbid, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxObjHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxObjHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Ask questions on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xSynchronous">When set to true, outputs response as JSON in a non-streaming way. This is slower and requires waiting for entire answer to be ready.</param>
    /// <param name="body">Request body</param>
    [Description("Ask questions on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> AskKnowledgeboxEndpointKbKbidAsk(string kbid, AskRequest body, string xNdbClient = "api", bool xShowConsumption = false, string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AskKnowledgeboxEndpointKbKbidAskAsync(kbid, body, xNdbClient, xShowConsumption, xNucliadbUser ?? "", xForwardedFor ?? "", xSynchronous, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to search for</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`filters`, `range_*`, `with_status`.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="sortField">Field to sort results with (Score not supported in catalog)</param>
    /// <param name="sortLimit">sortLimit</param>
    /// <param name="sortOrder">Order to sort results with</param>
    /// <param name="pageNumber">The page number of the results to return</param>
    /// <param name="pageSize">The number of results to return per page. The maximum number of results per page allowed is 200.</param>
    /// <param name="withStatus">Filter results by resource processing status</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="hidden">Set to filter only hidden or only non-hidden resources. Default is to return everything</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    [Description("List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> CatalogGetKbKbidCatalog(string kbid, string? query = null, object? filterExpression = null, List<string>? filters = null, List<string>? faceted = null, string? sortField = null, object? sortLimit = null, string sortOrder = "desc", int pageNumber = 0, int pageSize = 20, object? withStatus = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, object? hidden = null, List<string>? show = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CatalogGetKbKbidCatalogAsync(kbid, query ?? "", filterExpression, filters, faceted, sortField ?? "", sortLimit, sortOrder, pageNumber, pageSize, withStatus, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, hidden, show, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [Description("List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> CatalogPostKbKbidCatalog(string kbid, CatalogRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CatalogPostKbKbidCatalogAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Update current configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [Description("Update current configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> ConfigurationKbKbidConfigurationPatch(string kbid, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ConfigurationKbKbidConfigurationPatchAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Create configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [Description("Create configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> SetConfigurationKbKbidConfiguration(string kbid, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SetConfigurationKbKbidConfigurationAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Summary of amount of different things inside a knowledgebox --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [Description("Summary of amount of different things inside a knowledgebox --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> KnowledgeboxCountersKbKbidCounters(string kbid, bool debug = false, string xNUCLIADBROLES = "READER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KnowledgeboxCountersKbKbidCountersAsync(kbid, debug, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxCountersHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxCountersHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> StartKbExportEndpointKbKbidExport(string kbid, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.StartKbExportEndpointKbKbidExportAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkCreateExportResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorCreateExportResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="exportId">exportId</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> DownloadExportKbEndpointKbKbidExportExportId(string kbid, string exportId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadExportKbEndpointKbKbidExportExportIdAsync(kbid, exportId, CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="exportId">exportId</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> ExportStatusEndpointKbKbidExportExportIdStatusGet(string kbid, string exportId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ExportStatusEndpointKbKbidExportExportIdStatusGetAsync(kbid, exportId, CancellationToken.None);

        return result switch
        {
            OkStatusResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorStatusResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Send feedback for a search operation in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Send feedback for a search operation in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SendFeedbackEndpointKbKbidFeedback(string kbid, FeedbackRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SendFeedbackEndpointKbKbidFeedbackAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to search for</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`fields`, `filters`, `range_*`, `resource_filters`, `keyword_filters`.</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="topK">The number of results search should return. The maximum number of results allowed is 200.</param>
    /// <param name="minScore">Minimum similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreSemantic">Minimum semantic similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreBm25">Minimum bm25 score to filter paragraph and document index results</param>
    /// <param name="vectorset">Vectors index to perform the search in. If not provided, NucliaDB will use the default one</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="features">List of search features to use. Each value corresponds to a lookup into on of the different indexes</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="fieldType">Define which field types are serialized on resources of search results</param>
    /// <param name="extracted">[Deprecated] Please use GET resource endpoint instead to get extracted metadata</param>
    /// <param name="withDuplicates">Whether to return duplicate paragraphs on the same document</param>
    /// <param name="withSynonyms">Whether to return matches for custom knowledge box synonyms of the query terms. Note: only supported for `keyword` and `fulltext` search options.</param>
    /// <param name="autofilter">If set to true, the search will automatically add filters to the query. For example, it will filter results containing the entities detected in the query</param>
    /// <param name="securityGroups">List of security groups to filter search results for. Only resources matching the query and containing the specified security groups will be returned. If empty, all resources will be considered for the search.</param>
    /// <param name="showHidden">If set to false (default), excludes hidden resources from search</param>
    /// <param name="rankFusion">Rank fusion algorithm to use to merge results from multiple retrievers (keyword, semantic)</param>
    /// <param name="reranker">Reranker let you specify which method you want to use to rerank your results at the end of retrieval</param>
    /// <param name="searchConfiguration">Load find parameters from this configuration. Parameters in the request override parameters from the configuration.</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [Description("Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> FindKnowledgeboxKbKbidFind(string kbid, string? query = null, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, object? topK = null, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string rankFusion = "rrf", object? reranker = null, object? searchConfiguration = null, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.FindKnowledgeboxKbKbidFindAsync(kbid, query ?? "", filterExpression, fields, filters, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, rankFusion, reranker, searchConfiguration, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxFindResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxFindResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> FindPostKnowledgeboxKbKbidFind(string kbid, FindRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.FindPostKnowledgeboxKbKbidFindAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxFindResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxFindResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> GraphSearchKnowledgeboxKbKbidGraph(string kbid, GraphSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GraphSearchKnowledgeboxKbKbidGraphAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkGraphSearchResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphSearchResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve nodes (vertices) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Search on the Knowledge Box graph and retrieve nodes (vertices) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> GraphNodesSearchKnowledgeboxKbKbidGraphNodes(string kbid, GraphNodesSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GraphNodesSearchKnowledgeboxKbKbidGraphNodesAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkGraphNodesSearchResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphNodesSearchResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve relations (edges) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Search on the Knowledge Box graph and retrieve relations (edges) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> GraphRelationsSearchKnowledgeboxKbKbidGraphRelations(string kbid, GraphRelationsSearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GraphRelationsSearchKnowledgeboxKbKbidGraphRelationsAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkGraphRelationsSearchResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphRelationsSearchResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> StartKbImportEndpointKbKbidImport(string kbid, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.StartKbImportEndpointKbKbidImportAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkCreateImportResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorCreateImportResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="importId">importId</param>
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> ImportStatusEndpointKbKbidImportImportIdStatusGet(string kbid, string importId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ImportStatusEndpointKbKbidImportImportIdStatusGetAsync(kbid, importId, CancellationToken.None);

        return result switch
        {
            OkStatusResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorStatusResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="endpoint">endpoint</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> PredictProxyEndpointKbKbidPredictEndpoint(string kbid, string endpoint, object body, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.PredictProxyEndpointKbKbidPredictEndpointAsync(kbid, endpoint, body, xNucliadbUser ?? "", xNdbClient, xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="endpoint">endpoint</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [Description("Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> PredictProxyEndpointKbKbidPredictEndpoint2(string kbid, string endpoint, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.PredictProxyEndpointKbKbidPredictEndpointAsync2(kbid, endpoint, xNucliadbUser ?? "", xNdbClient, xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xSynchronous">When set to true, outputs response as JSON in a non-streaming way. This is slower and requires waiting for entire answer to be ready.</param>
    /// <param name="body">Request body</param>
    [Description("Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceAskEndpointByUuidKbKbidResourceRidAsk(string kbid, string rid, AskRequest body, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceAskEndpointByUuidKbKbidResourceRidAskAsync(kbid, rid, body, xShowConsumption, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", xSynchronous, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on a single resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="query">query</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`fields`, `filters`, `range_*`, `resource_filters`, `keyword_filters`.</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="sortField">Field to sort results with (Score not supported in catalog)</param>
    /// <param name="sortOrder">Order to sort results with</param>
    /// <param name="topK">The number of results search should return. The maximum number of results allowed is 200.</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="xNdbClient">xNdbClient</param>
    [Description("Search on a single resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceSearchKbKbidResourceRidSearch(string kbid, string rid, string query, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? sortField = null, string sortOrder = "desc", object? topK = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, bool highlight = false, bool debug = false, string xNdbClient = "api")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceSearchKbKbidResourceRidSearchAsync(kbid, rid, query, filterExpression, fields, filters, faceted, sortField, sortOrder, topK, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, xNdbClient, CancellationToken.None);

        return result switch
        {
            OkResourceSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to search for</param>
    /// <param name="filterExpression">Returns only documents that match this filter expression.Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters This allows building complex filtering expressions and replaces the following parameters:`fields`, `filters`, `range_*`, `resource_filters`, `keyword_filters`.</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="sortField">Field to sort results with (Score not supported in catalog)</param>
    /// <param name="sortLimit">sortLimit</param>
    /// <param name="sortOrder">Order to sort results with</param>
    /// <param name="topK">The number of results search should return. The maximum number of results allowed is 200.</param>
    /// <param name="minScore">Minimum similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreSemantic">Minimum semantic similarity score to filter vector index results. If not specified, the default minimum score of the semantic model associated to the Knowledge Box will be used. Check out the documentation for more information on how to use this parameter: https://docs.nuclia.dev/docs/rag/advanced/search#minimum-score</param>
    /// <param name="minScoreBm25">Minimum bm25 score to filter paragraph and document index results</param>
    /// <param name="vectorset">Vectors index to perform the search in. If not provided, NucliaDB will use the default one</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="features">List of search features to use. Each value corresponds to a lookup into on of the different indexes</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="fieldType">Define which field types are serialized on resources of search results</param>
    /// <param name="extracted">[Deprecated] Please use GET resource endpoint instead to get extracted metadata</param>
    /// <param name="withDuplicates">Whether to return duplicate paragraphs on the same document</param>
    /// <param name="withSynonyms">Whether to return matches for custom knowledge box synonyms of the query terms. Note: only supported for `keyword` and `fulltext` search options.</param>
    /// <param name="autofilter">If set to true, the search will automatically add filters to the query. For example, it will filter results containing the entities detected in the query</param>
    /// <param name="securityGroups">List of security groups to filter search results for. Only resources matching the query and containing the specified security groups will be returned. If empty, all resources will be considered for the search.</param>
    /// <param name="showHidden">If set to false (default), excludes hidden resources from search</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [Description("Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SearchKnowledgeboxKbKbidSearch(string kbid, string? query = null, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, string? sortField = null, object? sortLimit = null, string sortOrder = "desc", int topK = 20, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SearchKnowledgeboxKbKbidSearchAsync(kbid, query ?? "", filterExpression, fields, filters, faceted, sortField ?? "", sortLimit, sortOrder, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [Description("Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SearchPostKnowledgeboxKbKbidSearch(string kbid, SearchRequest body, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SearchPostKnowledgeboxKbKbidSearchAsync(kbid, body, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="slug">slug</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xSynchronous">When set to true, outputs response as JSON in a non-streaming way. This is slower and requires waiting for entire answer to be ready.</param>
    /// <param name="body">Request body</param>
    [Description("Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceAskEndpointBySlugKbKbidSlugSlugAsk(string kbid, string slug, AskRequest body, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceAskEndpointBySlugKbKbidSlugSlugAskAsync(kbid, slug, body, xShowConsumption, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", xSynchronous, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Suggestions on a knowledge box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="query">The query to get suggestions for</param>
    /// <param name="fields">The list of fields to search in. For instance: `a/title` to search only on title field. For more details on filtering by field, see: https://docs.nuclia.dev/docs/rag/advanced/search/#search-in-a-specific-field.</param>
    /// <param name="filters">The list of filters to apply. Filtering examples can be found here: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="faceted">The list of facets to calculate. The facets follow the same syntax as filters: https://docs.nuclia.dev/docs/rag/advanced/search-filters</param>
    /// <param name="rangeCreationStart">Resources created before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeCreationEnd">Resources created after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationStart">Resources modified before this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="rangeModificationEnd">Resources modified after this date will be filtered out of search results. Datetime are represented as a str in ISO 8601 format, like: 2008-09-15T15:53:00+05:00.</param>
    /// <param name="features">Features enabled for the suggest endpoint.</param>
    /// <param name="show">Controls which types of metadata are serialized on resources of search results</param>
    /// <param name="fieldType">Define which field types are serialized on resources of search results</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="highlight">If set to true, the query terms will be highlighted in the results between <mark>...</mark> tags</param>
    /// <param name="showHidden">If set to false (default), excludes hidden resources from search</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [Description("Suggestions on a knowledge box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SuggestKnowledgeboxKbKbidSuggest(string kbid, string query, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, List<string>? show = null, List<string>? fieldType = null, bool debug = false, bool highlight = false, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SuggestKnowledgeboxKbKbidSuggestAsync(kbid, query, fields, filters, faceted, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, show, fieldType, debug, highlight, showHidden, xNdbClient, xNucliadbUser ?? "", xForwardedFor ?? "", CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSuggestResultsHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSuggestResultsHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Summarize Your Documents --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="body">Request body</param>
    [Description("Summarize Your Documents --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SummarizeEndpointKbKbidSummarize(string kbid, SummarizeRequest body, bool xShowConsumption = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SummarizeEndpointKbKbidSummarizeAsync(kbid, body, xShowConsumption, CancellationToken.None);

        return result switch
        {
            OkSummarizedResponseHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSummarizedResponseHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Upload a file onto a Knowledge Box, field id will be file and rid will be autogenerated. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xFilename">Name of the file being uploaded.</param>
    /// <param name="xPassword">If the file is password protected, the password must be provided here.</param>
    /// <param name="xLanguage">xLanguage</param>
    /// <param name="xMd5">MD5 hash of the file being uploaded. This is used to check if the file has been uploaded before.</param>
    /// <param name="xExtractStrategy">Extract strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="xSplitStrategy">Split strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="body">Request body</param>
    [Description("Upload a file onto a Knowledge Box, field id will be file and rid will be autogenerated. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadKbKbidUpload(string kbid, object body, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadKbKbidUploadAsync(kbid, body, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, CancellationToken.None);

        return result switch
        {
            OkResourceFileUploadedHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFileUploadedHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Create a new knowledge box</summary>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    /// <param name="body">Request body</param>
    [Description("Create a new knowledge box")]
    public async Task<string> CreateKnowledgeBoxKbs(object body, string xNUCLIADBROLES = "MANAGER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CreateKnowledgeBoxKbsAsync(body, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxObj(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxObj(var httpError) => httpError switch
            {
                HttpError<string>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<string>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Get jsonschema definition for `learning_configuration` field of knowledgebox creation payload</summary>
    
    [Description("Get jsonschema definition for `learning_configuration` field of knowledgebox creation payload")]
    public async Task<string> LearningConfigurationSchemaLearningConfigurationSchema()
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.LearningConfigurationSchemaLearningConfigurationSchemaAsync(CancellationToken.None);

        return result switch
        {
            OkobjectHTTPValidationError(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectHTTPValidationError(var httpError) => httpError switch
            {
                HttpError<HTTPValidationError>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<HTTPValidationError>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }
}