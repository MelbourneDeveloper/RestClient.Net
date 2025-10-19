#nullable enable
using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol;
using Outcome;
using NucliaDB.Generated;

namespace NucliaDB.Mcp;

/// <summary>MCP server tools for NucliaDb API.</summary>
[McpServerToolType]
public class NucliaDbTools(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="slug">slug</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> KbBySlugKbSSlugGet(string slug)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KbBySlugKbSSlugGetAsync(slug, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxObj(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxObj(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> KbKbKbidGet(string kbid, string xNUCLIADBROLES = "READER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KbKbKbidGetAsync(kbid, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxObj(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxObj(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Ask questions on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> AskKnowledgeboxEndpointKbKbidAsk(string kbid, string xNdbClient = "api", bool xShowConsumption = false, string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false, AskRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AskKnowledgeboxEndpointKbKbidAskAsync(kbid, xNdbClient, xShowConsumption, xNucliadbUser, xForwardedFor, xSynchronous, body, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> CatalogGetKbKbidCatalog(string kbid, string? query = null, object? filterExpression = null, List<string>? filters = null, List<string>? faceted = null, string? sortField = null, object? sortLimit = null, string sortOrder = "desc", int pageNumber = 0, int pageSize = 20, object? withStatus = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, object? hidden = null, List<string>? show = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CatalogGetKbKbidCatalogAsync(kbid, query, filterExpression, filters, faceted, sortField, sortLimit, sortOrder, pageNumber, pageSize, withStatus, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, hidden, show, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("List resources of a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> CatalogPostKbKbidCatalog(string kbid, CatalogRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CatalogPostKbKbidCatalogAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Current configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Current configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> ConfigurationKbKbidConfigurationGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ConfigurationKbKbidConfigurationGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Update current configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Update current configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> ConfigurationKbKbidConfigurationPatch(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ConfigurationKbKbidConfigurationPatchAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Create configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Create configuration of models assigned to a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> SetConfigurationKbKbidConfiguration(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SetConfigurationKbKbidConfigurationAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Summary of amount of different things inside a knowledgebox --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="debug">If set, the response will include some extra metadata for debugging purposes, like the list of queried nodes.</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [McpTool]
    [Description("Summary of amount of different things inside a knowledgebox --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> KnowledgeboxCountersKbKbidCounters(string kbid, bool debug = false, string xNUCLIADBROLES = "READER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KnowledgeboxCountersKbKbidCountersAsync(kbid, debug, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxCounters(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxCounters(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> SetCustomSynonymsKbKbidCustomSynonyms(string kbid, KnowledgeBoxSynonyms body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SetCustomSynonymsKbKbidCustomSynonymsAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> CustomSynonymsKbKbidCustomSynonymsDelete(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CustomSynonymsKbKbidCustomSynonymsDeleteAsync(kbid, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> CustomSynonymsKbKbidCustomSynonymsGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CustomSynonymsKbKbidCustomSynonymsGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxSynonyms(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxSynonyms(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="group">group</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UpdateEntitiesGroupKbKbidEntitiesgroupGroup(string kbid, string group, UpdateEntitiesGroupPayload body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UpdateEntitiesGroupKbKbidEntitiesgroupGroupAsync(kbid, group, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="group">group</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> EntitiesKbKbidEntitiesgroupGroupDelete(string kbid, string group)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.EntitiesKbKbidEntitiesgroupGroupDeleteAsync(kbid, group, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="group">group</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> EntityKbKbidEntitiesgroupGroupGet(string kbid, string group)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.EntityKbKbidEntitiesgroupGroupGetAsync(kbid, group, CancellationToken.None);

        return result switch
        {
            OkEntitiesGroup(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorEntitiesGroup(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> CreateEntitiesGroupKbKbidEntitiesgroups(string kbid, CreateEntitiesGroupPayload body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CreateEntitiesGroupKbKbidEntitiesgroupsAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="showEntities">showEntities</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> EntitiesKbKbidEntitiesgroupsGet(string kbid, bool showEntities = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.EntitiesKbKbidEntitiesgroupsGetAsync(kbid, showEntities, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxEntities(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxEntities(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> StartKbExportEndpointKbKbidExport(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.StartKbExportEndpointKbKbidExportAsync(kbid, CancellationToken.None);

        return result switch
        {
            OkCreateExportResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorCreateExportResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="exportId">exportId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> DownloadExportKbEndpointKbKbidExportExportId(string kbid, string exportId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadExportKbEndpointKbKbidExportExportIdAsync(kbid, exportId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="exportId">exportId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> ExportStatusEndpointKbKbidExportExportIdStatusGet(string kbid, string exportId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ExportStatusEndpointKbKbidExportExportIdStatusGetAsync(kbid, exportId, CancellationToken.None);

        return result switch
        {
            OkStatusResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorStatusResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Add a extract strategy to a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Add a extract strategy to a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> AddStrategyKbKbidExtractStrategies(string kbid, ExtractConfig body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddStrategyKbKbidExtractStrategiesAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get available extract strategies --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Get available extract strategies --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> ExtractStrategiesKbKbidExtractStrategiesGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ExtractStrategiesKbKbidExtractStrategiesGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Removes a extract strategy from a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="strategyId">strategyId</param>
    [McpTool]
    [Description("Removes a extract strategy from a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> StrategyKbKbidExtractStrategiesStrategyStrategyIdDelete(string kbid, string strategyId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.StrategyKbKbidExtractStrategiesStrategyStrategyIdDeleteAsync(kbid, strategyId, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get extract strategy for a given id --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="strategyId">strategyId</param>
    [McpTool]
    [Description("Get extract strategy for a given id --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> ExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet(string kbid, string strategyId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGetAsync(kbid, strategyId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Send feedback for a search operation in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Send feedback for a search operation in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SendFeedbackEndpointKbKbidFeedback(string kbid, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, FeedbackRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SendFeedbackEndpointKbKbidFeedbackAsync(kbid, xNdbClient, xNucliadbUser, xForwardedFor, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> FindKnowledgeboxKbKbidFind(string kbid, string? query = null, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, object? topK = null, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string rankFusion = "rrf", object? reranker = null, object? searchConfiguration = null, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.FindKnowledgeboxKbKbidFindAsync(kbid, query, filterExpression, fields, filters, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, rankFusion, reranker, searchConfiguration, xNdbClient, xNucliadbUser, xForwardedFor, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxFindResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxFindResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Find on a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> FindPostKnowledgeboxKbKbidFind(string kbid, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, FindRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.FindPostKnowledgeboxKbKbidFindAsync(kbid, xNdbClient, xNucliadbUser, xForwardedFor, body, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxFindResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxFindResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Search on the Knowledge Box graph and retrieve triplets of vertex-edge-vertex --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> GraphSearchKnowledgeboxKbKbidGraph(string kbid, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, GraphSearchRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GraphSearchKnowledgeboxKbKbidGraphAsync(kbid, xNdbClient, xNucliadbUser, xForwardedFor, body, CancellationToken.None);

        return result switch
        {
            OkGraphSearchResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphSearchResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve nodes (vertices) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Search on the Knowledge Box graph and retrieve nodes (vertices) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> GraphNodesSearchKnowledgeboxKbKbidGraphNodes(string kbid, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, GraphNodesSearchRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GraphNodesSearchKnowledgeboxKbKbidGraphNodesAsync(kbid, xNdbClient, xNucliadbUser, xForwardedFor, body, CancellationToken.None);

        return result switch
        {
            OkGraphNodesSearchResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphNodesSearchResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Search on the Knowledge Box graph and retrieve relations (edges) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Search on the Knowledge Box graph and retrieve relations (edges) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> GraphRelationsSearchKnowledgeboxKbKbidGraphRelations(string kbid, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, GraphRelationsSearchRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GraphRelationsSearchKnowledgeboxKbKbidGraphRelationsAsync(kbid, xNdbClient, xNucliadbUser, xForwardedFor, body, CancellationToken.None);

        return result switch
        {
            OkGraphRelationsSearchResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGraphRelationsSearchResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> StartKbImportEndpointKbKbidImport(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.StartKbImportEndpointKbKbidImportAsync(kbid, CancellationToken.None);

        return result switch
        {
            OkCreateImportResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorCreateImportResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="importId">importId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `READER`")]
    public async Task<string> ImportStatusEndpointKbKbidImportImportIdStatusGet(string kbid, string importId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ImportStatusEndpointKbKbidImportImportIdStatusGetAsync(kbid, importId, CancellationToken.None);

        return result switch
        {
            OkStatusResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorStatusResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="labelset">labelset</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> SetLabelsetEndpointKbKbidLabelsetLabelset(string kbid, string labelset, LabelSet body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SetLabelsetEndpointKbKbidLabelsetLabelsetAsync(kbid, labelset, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="labelset">labelset</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> LabelsetEndpointKbKbidLabelsetLabelsetDelete(string kbid, string labelset)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.LabelsetEndpointKbKbidLabelsetLabelsetDeleteAsync(kbid, labelset, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="labelset">labelset</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> LabelsetEndpointKbKbidLabelsetLabelsetGet(string kbid, string labelset)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.LabelsetEndpointKbKbidLabelsetLabelsetGetAsync(kbid, labelset, CancellationToken.None);

        return result switch
        {
            OkLabelSet(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorLabelSet(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> LabelsetsEndointKbKbidLabelsetsGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.LabelsetsEndointKbKbidLabelsetsGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxLabels(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxLabels(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get metadata for a particular model --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="modelId">modelId</param>
    [McpTool]
    [Description("Get metadata for a particular model --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> ModelKbKbidModelModelIdGet(string kbid, string modelId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ModelKbKbidModelModelIdGetAsync(kbid, modelId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get available models --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Get available models --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> ModelsKbKbidModelsGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ModelsKbKbidModelsGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Download the trained model or any other generated file as a result of a training task on a Knowledge Box. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="modelId">modelId</param>
    /// <param name="filename">filename</param>
    [McpTool]
    [Description("Download the trained model or any other generated file as a result of a training task on a Knowledge Box. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> DownloadModelKbKbidModelsModelIdFilename(string kbid, string modelId, string filename)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadModelKbKbidModelsModelIdFilenameAsync(kbid, modelId, filename, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Provides a stream of activity notifications for the given Knowledge Box. The stream will be automatically closed after 2 minutes. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Provides a stream of activity notifications for the given Knowledge Box. The stream will be automatically closed after 2 minutes. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> NotificationsEndpointKbKbidNotifications(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.NotificationsEndpointKbKbidNotificationsAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="endpoint">endpoint</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [McpTool]
    [Description("Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> PredictProxyEndpointKbKbidPredictEndpoint(string kbid, string endpoint, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.PredictProxyEndpointKbKbidPredictEndpointAsync(kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="endpoint">endpoint</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [McpTool]
    [Description("Convenience endpoint that proxies requests to the Predict API. It adds the Knowledge Box configuration settings as headers to the predict API request. Refer to the Predict API documentation for more details about the request and response models: https://docs.nuclia.dev/docs/nua-api#tag/Predict --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> PredictProxyEndpointKbKbidPredictEndpoint2(string kbid, string endpoint, string? xNucliadbUser = null, string xNdbClient = "api", string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.PredictProxyEndpointKbKbidPredictEndpointAsync2(kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Provides the status of the processing of the given Knowledge Box. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="cursor">cursor</param>
    /// <param name="scheduled">scheduled</param>
    /// <param name="limit">limit</param>
    [McpTool]
    [Description("Provides the status of the processing of the given Knowledge Box. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ProcessingStatusKbKbidProcessingStatus(string kbid, object? cursor = null, object? scheduled = null, int limit = 20)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ProcessingStatusKbKbidProcessingStatusAsync(kbid, cursor, scheduled, limit, CancellationToken.None);

        return result switch
        {
            OkRequestsResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorRequestsResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="pathRid">pathRid</param>
    /// <param name="field">field</param>
    /// <param name="xExtractStrategy">Extract strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="xSplitStrategy">Split strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> TusPostRidPrefixKbKbidResourcePathRidFileFieldTusupload(string kbid, string pathRid, string field, object? xExtractStrategy = null, object? xSplitStrategy = null, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.TusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadAsync(kbid, pathRid, field, xExtractStrategy, xSplitStrategy, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="pathRid">pathRid</param>
    /// <param name="field">field</param>
    /// <param name="uploadId">uploadId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadId(string kbid, string pathRid, string field, string uploadId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdAsync(kbid, pathRid, field, uploadId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Upload a file as a field on an existing resource, if the field exists will return a conflict (419) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="pathRid">pathRid</param>
    /// <param name="field">field</param>
    /// <param name="xFilename">Name of the file being uploaded.</param>
    /// <param name="xPassword">If the file is password protected, the password must be provided here.</param>
    /// <param name="xLanguage">xLanguage</param>
    /// <param name="xMd5">MD5 hash of the file being uploaded. This is used to check if the file has been uploaded before.</param>
    /// <param name="xExtractStrategy">Extract strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="xSplitStrategy">Split strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    [McpTool]
    [Description("Upload a file as a field on an existing resource, if the field exists will return a conflict (419) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadRidPrefixKbKbidResourcePathRidFileFieldUpload(string kbid, string pathRid, string field, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadRidPrefixKbKbidResourcePathRidFileFieldUploadAsync(kbid, pathRid, field, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, CancellationToken.None);

        return result switch
        {
            OkResourceFileUploaded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFileUploaded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xSkipStore">If set to true, file fields will not be saved in the blob storage. They will only be sent to process.</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ModifyResourceRidPrefixKbKbidResourceRid(string kbid, string rid, string? xNucliadbUser = null, bool xSkipStore = false, string xNUCLIADBROLES = "WRITER", UpdateResourcePayload body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ModifyResourceRidPrefixKbKbidResourceRidAsync(kbid, rid, xNucliadbUser, xSkipStore, xNUCLIADBROLES, body, CancellationToken.None);

        return result switch
        {
            OkResourceUpdated(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceUpdated(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ResourceRidPrefixKbKbidResourceRidDelete(string kbid, string rid, string xNUCLIADBROLES = "WRITER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceRidPrefixKbKbidResourceRidDeleteAsync(kbid, rid, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="show">show</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="extracted">extracted</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceByUuidKbKbidResourceRidGet(string kbid, string rid, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, string? xNucliadbUser = null, string? xForwardedFor = null, string xNUCLIADBROLES = "READER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceByUuidKbKbidResourceRidGetAsync(kbid, rid, show, fieldType, extracted, xNucliadbUser, xForwardedFor, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkNucliadbModelsResourceResource(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorNucliadbModelsResourceResource(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceAskEndpointByUuidKbKbidResourceRidAsk(string kbid, string rid, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false, AskRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceAskEndpointByUuidKbKbidResourceRidAskAsync(kbid, rid, xShowConsumption, xNdbClient, xNucliadbUser, xForwardedFor, xSynchronous, body, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldId(string kbid, string rid, string fieldId, InputConversationField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdAsync(kbid, rid, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="messageId">messageId</param>
    /// <param name="fileNum">fileNum</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> DownloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNum(string kbid, string rid, string fieldId, string messageId, int fileNum)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumAsync(kbid, rid, fieldId, messageId, fileNum, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AppendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessages(string kbid, string rid, string fieldId, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AppendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesAsync(kbid, rid, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="xSkipStore">If set to true, file fields will not be saved in the blob storage. They will only be sent to process.</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldFileRidPrefixKbKbidResourceRidFileFieldId(string kbid, string rid, string fieldId, bool xSkipStore = false, FileField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdAsync(kbid, rid, fieldId, xSkipStore, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="inline">inline</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> DownloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadField(string kbid, string rid, string fieldId, bool inline = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldAsync(kbid, rid, fieldId, inline, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="resetTitle">Reset the title of the resource so that the file or link computed titles are set after processing.</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xFilePassword">If a file is password protected, the password must be provided here for the file to be processed</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ReprocessFileFieldKbKbidResourceRidFileFieldIdReprocess(string kbid, string rid, string fieldId, bool resetTitle = false, string? xNucliadbUser = null, object? xFilePassword = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ReprocessFileFieldKbKbidResourceRidFileFieldIdReprocessAsync(kbid, rid, fieldId, resetTitle, xNucliadbUser, xFilePassword, CancellationToken.None);

        return result switch
        {
            OkResourceUpdated(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceUpdated(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="field">field</param>
    /// <param name="uploadId">uploadId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> TusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadId(string kbid, string rid, string field, string uploadId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.TusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdAsync(kbid, rid, field, uploadId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldId(string kbid, string rid, string fieldId, LinkField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdAsync(kbid, rid, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="reindexVectors">reindexVectors</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ReindexResourceRidPrefixKbKbidResourceRidReindex(string kbid, string rid, bool reindexVectors = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ReindexResourceRidPrefixKbKbidResourceRidReindexAsync(kbid, rid, reindexVectors, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="resetTitle">Reset the title of the resource so that the file or link computed titles are set after processing.</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ReprocessResourceRidPrefixKbKbidResourceRidReprocess(string kbid, string rid, bool resetTitle = false, string? xNucliadbUser = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ReprocessResourceRidPrefixKbKbidResourceRidReprocessAsync(kbid, rid, resetTitle, xNucliadbUser, CancellationToken.None);

        return result switch
        {
            OkResourceUpdated(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceUpdated(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Run Agents on Resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Run Agents on Resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> RunAgentsByUuidKbKbidResourceRidRunAgents(string kbid, string rid, string? xNucliadbUser = null, ResourceAgentsRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.RunAgentsByUuidKbKbidResourceRidRunAgentsAsync(kbid, rid, xNucliadbUser, body, CancellationToken.None);

        return result switch
        {
            OkResourceAgentsResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceAgentsResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Search on a single resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceSearchKbKbidResourceRidSearch(string kbid, string rid, string query, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? sortField = null, string sortOrder = "desc", object? topK = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, bool highlight = false, bool debug = false, string xNdbClient = "api")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceSearchKbKbidResourceRidSearchAsync(kbid, rid, query, filterExpression, fields, filters, faceted, sortField, sortOrder, topK, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, xNdbClient, CancellationToken.None);

        return result switch
        {
            OkResourceSearchResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceSearchResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldId(string kbid, string rid, string fieldId, string xNUCLIADBROLES = "WRITER", TextField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdAsync(kbid, rid, fieldId, xNUCLIADBROLES, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="fieldId">fieldId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete(string kbid, string rid, string fieldType, string fieldId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDeleteAsync(kbid, rid, fieldType, fieldId, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="show">show</param>
    /// <param name="extracted">extracted</param>
    /// <param name="page">page</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet(string kbid, string rid, string fieldType, string fieldId, List<string>? show = null, List<string>? extracted = null, object? page = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGetAsync(kbid, rid, fieldType, fieldId, show, extracted, page, CancellationToken.None);

        return result switch
        {
            OkResourceField(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceField(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="downloadField">downloadField</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> DownloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadField(string kbid, string rid, string fieldType, string fieldId, string downloadField)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldAsync(kbid, rid, fieldType, fieldId, downloadField, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Create a new Resource in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xSkipStore">If set to true, file fields will not be saved in the blob storage. They will only be sent to process.</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Create a new Resource in a Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> CreateResourceKbKbidResources(string kbid, bool xSkipStore = false, string? xNucliadbUser = null, string xNUCLIADBROLES = "WRITER", CreateResourcePayload body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CreateResourceKbKbidResourcesAsync(kbid, xSkipStore, xNucliadbUser, xNUCLIADBROLES, body, CancellationToken.None);

        return result switch
        {
            OkResourceCreated(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceCreated(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>List of resources of a knowledgebox --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="page">Requested page number (0-based)</param>
    /// <param name="size">Page size</param>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    [McpTool]
    [Description("List of resources of a knowledgebox --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ListResourcesKbKbidResources(string kbid, int page = 0, int size = 20, string xNUCLIADBROLES = "READER")
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ListResourcesKbKbidResourcesAsync(kbid, page, size, xNUCLIADBROLES, CancellationToken.None);

        return result switch
        {
            OkResourceList(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceList(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get jsonschema definition to update the `learning_configuration` of your Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Get jsonschema definition to update the `learning_configuration` of your Knowledge Box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> SchemaForConfigurationUpdatesKbKbidSchemaGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SchemaForConfigurationUpdatesKbKbidSchemaGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SearchKnowledgeboxKbKbidSearch(string kbid, string? query = null, object? filterExpression = null, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, string? sortField = null, object? sortLimit = null, string sortOrder = "desc", int topK = 20, object? minScore = null, object? minScoreSemantic = null, float minScoreBm25 = 0, object? vectorset = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, bool debug = false, bool highlight = false, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, bool withDuplicates = false, bool withSynonyms = false, bool autofilter = false, List<string>? securityGroups = null, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SearchKnowledgeboxKbKbidSearchAsync(kbid, query, filterExpression, fields, filters, faceted, sortField, sortLimit, sortOrder, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, xNdbClient, xNucliadbUser, xForwardedFor, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xNdbClient">xNdbClient</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Search on a Knowledge Box and retrieve separate results for documents, paragraphs, and sentences. Usually, it is better to use `find` --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SearchPostKnowledgeboxKbKbidSearch(string kbid, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, SearchRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SearchPostKnowledgeboxKbKbidSearchAsync(kbid, xNdbClient, xNucliadbUser, xForwardedFor, body, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSearchResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSearchResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ListSearchConfigurationsKbKbidSearchConfigurations(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ListSearchConfigurationsKbKbidSearchConfigurationsAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="configName">configName</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> CreateSearchConfigurationKbKbidSearchConfigurationsConfigName(string kbid, string configName, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CreateSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync(kbid, configName, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="configName">configName</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UpdateSearchConfigurationKbKbidSearchConfigurationsConfigName(string kbid, string configName, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UpdateSearchConfigurationKbKbidSearchConfigurationsConfigNameAsync(kbid, configName, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="configName">configName</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> SearchConfigurationKbKbidSearchConfigurationsConfigNameDelete(string kbid, string configName)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SearchConfigurationKbKbidSearchConfigurationsConfigNameDeleteAsync(kbid, configName, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="configName">configName</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SearchConfigurationKbKbidSearchConfigurationsConfigNameGet(string kbid, string configName)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SearchConfigurationKbKbidSearchConfigurationsConfigNameGetAsync(kbid, configName, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="xSkipStore">If set to true, file fields will not be saved in the blob storage. They will only be sent to process.</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ModifyResourceRslugPrefixKbKbidSlugRslug(string kbid, string rslug, bool xSkipStore = false, string? xNucliadbUser = null, UpdateResourcePayload body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ModifyResourceRslugPrefixKbKbidSlugRslugAsync(kbid, rslug, xSkipStore, xNucliadbUser, body, CancellationToken.None);

        return result switch
        {
            OkResourceUpdated(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceUpdated(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ResourceRslugPrefixKbKbidSlugRslugDelete(string kbid, string rslug)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceRslugPrefixKbKbidSlugRslugDeleteAsync(kbid, rslug, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="show">show</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="extracted">extracted</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="xForwardedFor">xForwardedFor</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceBySlugKbKbidSlugRslugGet(string kbid, string rslug, List<string>? show = null, List<string>? fieldType = null, List<string>? extracted = null, string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceBySlugKbKbidSlugRslugGetAsync(kbid, rslug, show, fieldType, extracted, xNucliadbUser, xForwardedFor, CancellationToken.None);

        return result switch
        {
            OkNucliadbModelsResourceResource(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorNucliadbModelsResourceResource(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldId(string kbid, string rslug, string fieldId, InputConversationField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdAsync(kbid, rslug, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="messageId">messageId</param>
    /// <param name="fileNum">fileNum</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> DownloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNum(string kbid, string rslug, string fieldId, string messageId, int fileNum)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumAsync(kbid, rslug, fieldId, messageId, fileNum, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AppendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessages(string kbid, string rslug, string fieldId, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AppendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesAsync(kbid, rslug, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="xSkipStore">If set to true, file fields will not be saved in the blob storage. They will only be sent to process.</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldId(string kbid, string rslug, string fieldId, bool xSkipStore = false, FileField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdAsync(kbid, rslug, fieldId, xSkipStore, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="inline">inline</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> DownloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadField(string kbid, string rslug, string fieldId, bool inline = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldAsync(kbid, rslug, fieldId, inline, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="field">field</param>
    /// <param name="xExtractStrategy">Extract strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="xSplitStrategy">Split strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> TusPostRslugPrefixKbKbidSlugRslugFileFieldTusupload(string kbid, string rslug, string field, object? xExtractStrategy = null, object? xSplitStrategy = null, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.TusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadAsync(kbid, rslug, field, xExtractStrategy, xSplitStrategy, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="field">field</param>
    /// <param name="uploadId">uploadId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> TusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadId(string kbid, string rslug, string field, string uploadId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.TusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdAsync(kbid, rslug, field, uploadId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="field">field</param>
    /// <param name="uploadId">uploadId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadInformationKbKbidSlugRslugFileFieldTusuploadUploadId(string kbid, string rslug, string field, string uploadId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdAsync(kbid, rslug, field, uploadId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Upload a file as a field on an existing resource, if the field exists will return a conflict (419) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="field">field</param>
    /// <param name="xFilename">Name of the file being uploaded.</param>
    /// <param name="xPassword">If the file is password protected, the password must be provided here.</param>
    /// <param name="xLanguage">xLanguage</param>
    /// <param name="xMd5">MD5 hash of the file being uploaded. This is used to check if the file has been uploaded before.</param>
    /// <param name="xExtractStrategy">Extract strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="xSplitStrategy">Split strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    [McpTool]
    [Description("Upload a file as a field on an existing resource, if the field exists will return a conflict (419) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadRslugPrefixKbKbidSlugRslugFileFieldUpload(string kbid, string rslug, string field, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadRslugPrefixKbKbidSlugRslugFileFieldUploadAsync(kbid, rslug, field, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, CancellationToken.None);

        return result switch
        {
            OkResourceFileUploaded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFileUploaded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldId(string kbid, string rslug, string fieldId, LinkField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdAsync(kbid, rslug, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="reindexVectors">reindexVectors</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ReindexResourceRslugPrefixKbKbidSlugRslugReindex(string kbid, string rslug, bool reindexVectors = false)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ReindexResourceRslugPrefixKbKbidSlugRslugReindexAsync(kbid, rslug, reindexVectors, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="resetTitle">Reset the title of the resource so that the file or link computed titles are set after processing.</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ReprocessResourceRslugPrefixKbKbidSlugRslugReprocess(string kbid, string rslug, bool resetTitle = false, string? xNucliadbUser = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ReprocessResourceRslugPrefixKbKbidSlugRslugReprocessAsync(kbid, rslug, resetTitle, xNucliadbUser, CancellationToken.None);

        return result switch
        {
            OkResourceUpdated(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceUpdated(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> AddResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldId(string kbid, string rslug, string fieldId, TextField body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdAsync(kbid, rslug, fieldId, body, CancellationToken.None);

        return result switch
        {
            OkResourceFieldAdded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFieldAdded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="fieldId">fieldId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> ResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete(string kbid, string rslug, string fieldType, string fieldId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDeleteAsync(kbid, rslug, fieldType, fieldId, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="show">show</param>
    /// <param name="extracted">extracted</param>
    /// <param name="page">page</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet(string kbid, string rslug, string fieldType, string fieldId, List<string>? show = null, List<string>? extracted = null, object? page = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGetAsync(kbid, rslug, fieldType, fieldId, show, extracted, page, CancellationToken.None);

        return result switch
        {
            OkResourceField(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceField(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="fieldType">fieldType</param>
    /// <param name="fieldId">fieldId</param>
    /// <param name="downloadField">downloadField</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> DownloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadField(string kbid, string rslug, string fieldType, string fieldId, string downloadField)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DownloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldAsync(kbid, rslug, fieldType, fieldId, downloadField, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Ask questions to a resource --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> ResourceAskEndpointBySlugKbKbidSlugSlugAsk(string kbid, string slug, bool xShowConsumption = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null, bool xSynchronous = false, AskRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.ResourceAskEndpointBySlugKbKbidSlugSlugAskAsync(kbid, slug, xShowConsumption, xNdbClient, xNucliadbUser, xForwardedFor, xSynchronous, body, CancellationToken.None);

        return result switch
        {
            OkSyncAskResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSyncAskResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Run Agents on Resource (by slug) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="slug">slug</param>
    /// <param name="xNucliadbUser">xNucliadbUser</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Run Agents on Resource (by slug) --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> RunAgentsBySlugKbKbidSlugSlugRunAgents(string kbid, string slug, string? xNucliadbUser = null, ResourceAgentsRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.RunAgentsBySlugKbKbidSlugSlugRunAgentsAsync(kbid, slug, xNucliadbUser, body, CancellationToken.None);

        return result switch
        {
            OkResourceAgentsResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceAgentsResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Add a split strategy to a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Add a split strategy to a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> AddSplitStrategyKbKbidSplitStrategies(string kbid, SplitConfiguration body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.AddSplitStrategyKbKbidSplitStrategiesAsync(kbid, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get available split strategies --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    [McpTool]
    [Description("Get available split strategies --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> SplitStrategiesKbKbidSplitStrategiesGet(string kbid)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SplitStrategiesKbKbidSplitStrategiesGetAsync(kbid, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Removes a split strategy from a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="strategyId">strategyId</param>
    [McpTool]
    [Description("Removes a split strategy from a KB --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `MANAGER` - `WRITER`")]
    public async Task<string> SplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete(string kbid, string strategyId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDeleteAsync(kbid, strategyId, CancellationToken.None);

        return result switch
        {
            OkUnit(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorUnit(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get split strategy for a given id --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="strategyId">strategyId</param>
    [McpTool]
    [Description("Get split strategy for a given id --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER` - `MANAGER`")]
    public async Task<string> SplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet(string kbid, string strategyId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGetAsync(kbid, strategyId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Suggestions on a knowledge box --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SuggestKnowledgeboxKbKbidSuggest(string kbid, string query, List<string>? fields = null, List<string>? filters = null, List<string>? faceted = null, object? rangeCreationStart = null, object? rangeCreationEnd = null, object? rangeModificationStart = null, object? rangeModificationEnd = null, List<string>? features = null, List<string>? show = null, List<string>? fieldType = null, bool debug = false, bool highlight = false, bool showHidden = false, string xNdbClient = "api", string? xNucliadbUser = null, string? xForwardedFor = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SuggestKnowledgeboxKbKbidSuggestAsync(kbid, query, fields, filters, faceted, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, show, fieldType, debug, highlight, showHidden, xNdbClient, xNucliadbUser, xForwardedFor, CancellationToken.None);

        return result switch
        {
            OkKnowledgeboxSuggestResults(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeboxSuggestResults(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Summarize Your Documents --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xShowConsumption">xShowConsumption</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Summarize Your Documents --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `READER`")]
    public async Task<string> SummarizeEndpointKbKbidSummarize(string kbid, bool xShowConsumption = false, SummarizeRequest body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.SummarizeEndpointKbKbidSummarizeAsync(kbid, xShowConsumption, body, CancellationToken.None);

        return result switch
        {
            OkSummarizedResponse(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorSummarizedResponse(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="xExtractStrategy">Extract strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="xSplitStrategy">Split strategy to use when uploading a file. If not provided, the default strategy will be used.</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> TusPostKbKbidTusupload(string kbid, object? xExtractStrategy = null, object? xSplitStrategy = null, object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.TusPostKbKbidTusuploadAsync(kbid, xExtractStrategy, xSplitStrategy, body, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>TUS Server information</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="rid">rid</param>
    /// <param name="rslug">rslug</param>
    /// <param name="uploadId">uploadId</param>
    /// <param name="field">field</param>
    [McpTool]
    [Description("TUS Server information")]
    public async Task<string> TusOptionsKbKbidTusupload(string kbid, object? rid = null, object? rslug = null, object? uploadId = null, object? field = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.TusOptionsKbKbidTusuploadAsync(kbid, rid, rslug, uploadId, field, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="uploadId">uploadId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> KbKbidTusuploadUploadIdPatch(string kbid, string uploadId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.KbKbidTusuploadUploadIdPatchAsync(kbid, uploadId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`</summary>
    /// <param name="kbid">kbid</param>
    /// <param name="uploadId">uploadId</param>
    [McpTool]
    [Description("--- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadInformationKbKbidTusuploadUploadId(string kbid, string uploadId)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadInformationKbKbidTusuploadUploadIdAsync(kbid, uploadId, CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
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
    [McpTool]
    [Description("Upload a file onto a Knowledge Box, field id will be file and rid will be autogenerated. --- ## Authorization roles Authenticated user needs to fulfill one of this roles, otherwise the request will be rejected with a `403` response. - `WRITER`")]
    public async Task<string> UploadKbKbidUpload(string kbid, object? xFilename = null, object? xPassword = null, object? xLanguage = null, object? xMd5 = null, object? xExtractStrategy = null, object? xSplitStrategy = null)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.UploadKbKbidUploadAsync(kbid, xFilename, xPassword, xLanguage, xMd5, xExtractStrategy, xSplitStrategy, CancellationToken.None);

        return result switch
        {
            OkResourceFileUploaded(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResourceFileUploaded(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Create a new knowledge box</summary>
    /// <param name="xNUCLIADBROLES">xNUCLIADBROLES</param>
    /// <param name="body">Request body</param>
    [McpTool]
    [Description("Create a new knowledge box")]
    public async Task<string> CreateKnowledgeBoxKbs(string xNUCLIADBROLES = "MANAGER", object body)
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CreateKnowledgeBoxKbsAsync(xNUCLIADBROLES, body, CancellationToken.None);

        return result switch
        {
            OkKnowledgeBoxObj(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorKnowledgeBoxObj(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }

    /// <summary>Get jsonschema definition for `learning_configuration` field of knowledgebox creation payload</summary>
    
    [McpTool]
    [Description("Get jsonschema definition for `learning_configuration` field of knowledgebox creation payload")]
    public async Task<string> LearningConfigurationSchemaLearningConfigurationSchema()
    {
        var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.LearningConfigurationSchemaLearningConfigurationSchemaAsync(CancellationToken.None);

        return result switch
        {
            Okobject(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            Errorobject(var error) =>
                $"Error: {error.StatusCode} - {error.Body}",
            _ => "Unknown error"
        };
    }
}