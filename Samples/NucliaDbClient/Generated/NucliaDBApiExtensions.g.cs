using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RestClient.Net;
using RestClient.Net.Utilities;
using Outcome;
using Urls;

namespace NucliaDB.Generated;

/// <summary>Extension methods for API operations.</summary>
public static class NucliaDBApiExtensions
{
    #region Configuration

    private static readonly AbsoluteUrl BaseUrl = "https://{region-x}.stashify.cloud".ToAbsoluteUrl();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    #endregion

    #region Kb Operations

    /// <summary>Get Knowledge Box (by slug)</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<string>>> GetKbBySlugKbSSlugGet(
        this HttpClient httpClient,
        string slug,
        CancellationToken ct = default
    ) => _getKbBySlugKbSSlugGet(httpClient, slug, ct);
    
    /// <summary>Get Knowledge Box</summary>
    public static Task<Result<KnowledgeBoxObj, HttpError<string>>> GetKbKbKbidGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getKbKbKbidGet(httpClient, kbid, ct);
    
    /// <summary>Ask Knowledge Box</summary>
    public static Task<Result<SyncAskResponse, HttpError<string>>> AskKnowledgeboxEndpointKbKbidAskPost(
        this HttpClient httpClient,
        (string Params, AskRequest Body) param,
        CancellationToken ct = default
    ) => _askKnowledgeboxEndpointKbKbidAskPost(httpClient, param, ct);
    
    /// <summary>List resources of a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> CatalogGetKbKbidCatalogGet(
        this HttpClient httpClient,
        string kbid, string query, object filterExpression, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int pageNumber, int pageSize, object withStatus, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, object hidden, List<ResourceProperties> show,
        CancellationToken ct = default
    ) => _catalogGetKbKbidCatalogGet(httpClient, (kbid, query, filterExpression, filters, faceted, sortField, sortLimit, sortOrder, pageNumber, pageSize, withStatus, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, hidden, show), ct);
    
    /// <summary>List resources of a Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> CatalogPostKbKbidCatalogPost(
        this HttpClient httpClient,
        (string Params, CatalogRequest Body) param,
        CancellationToken ct = default
    ) => _catalogPostKbKbidCatalogPost(httpClient, param, ct);
    
    /// <summary>Get Knowledge Box models configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetConfigurationKbKbidConfigurationGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getConfigurationKbKbidConfigurationGet(httpClient, kbid, ct);
    
    /// <summary>Update Knowledge Box models configuration</summary>
    public static Task<Result<object, HttpError<string>>> PatchConfigurationKbKbidConfigurationPatch(
        this HttpClient httpClient,
        (string Params, object Body) param,
        CancellationToken ct = default
    ) => _patchConfigurationKbKbidConfigurationPatch(httpClient, param, ct);
    
    /// <summary>Create Knowledge Box models configuration</summary>
    public static Task<Result<object, HttpError<string>>> SetConfigurationKbKbidConfigurationPost(
        this HttpClient httpClient,
        (string Params, object Body) param,
        CancellationToken ct = default
    ) => _setConfigurationKbKbidConfigurationPost(httpClient, param, ct);
    
    /// <summary>Knowledgebox Counters</summary>
    public static Task<Result<KnowledgeboxCounters, HttpError<string>>> KnowledgeboxCountersKbKbidCountersGet(
        this HttpClient httpClient,
        string kbid, bool debug,
        CancellationToken ct = default
    ) => _knowledgeboxCountersKbKbidCountersGet(httpClient, (kbid, debug), ct);
    
    /// <summary>Set Knowledge Box Custom Synonyms</summary>
    public static Task<Result<object, HttpError<string>>> SetCustomSynonymsKbKbidCustomSynonymsPut(
        this HttpClient httpClient,
        (string Params, KnowledgeBoxSynonyms Body) param,
        CancellationToken ct = default
    ) => _setCustomSynonymsKbKbidCustomSynonymsPut(httpClient, param, ct);
    
    /// <summary>Delete Knowledge Box Custom Synonyms</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteCustomSynonymsKbKbidCustomSynonymsDelete(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _deleteCustomSynonymsKbKbidCustomSynonymsDelete(httpClient, kbid, ct);
    
    /// <summary>Get Knowledge Box Custom Synonyms</summary>
    public static Task<Result<KnowledgeBoxSynonyms, HttpError<string>>> GetCustomSynonymsKbKbidCustomSynonymsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getCustomSynonymsKbKbidCustomSynonymsGet(httpClient, kbid, ct);
    
    /// <summary>Update Knowledge Box Entities Group</summary>
    public static Task<Result<object, HttpError<string>>> UpdateEntitiesGroupKbKbidEntitiesgroupGroupPatch(
        this HttpClient httpClient,
        ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body) param,
        CancellationToken ct = default
    ) => _updateEntitiesGroupKbKbidEntitiesgroupGroupPatch(httpClient, param, ct);
    
    /// <summary>Delete Knowledge Box Entities</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteEntitiesKbKbidEntitiesgroupGroupDelete(
        this HttpClient httpClient,
        string kbid, string group,
        CancellationToken ct = default
    ) => _deleteEntitiesKbKbidEntitiesgroupGroupDelete(httpClient, (kbid, group), ct);
    
    /// <summary>Get a Knowledge Box Entities Group</summary>
    public static Task<Result<EntitiesGroup, HttpError<string>>> GetEntityKbKbidEntitiesgroupGroupGet(
        this HttpClient httpClient,
        string kbid, string group,
        CancellationToken ct = default
    ) => _getEntityKbKbidEntitiesgroupGroupGet(httpClient, (kbid, group), ct);
    
    /// <summary>Create Knowledge Box Entities Group</summary>
    public static Task<Result<object, HttpError<string>>> CreateEntitiesGroupKbKbidEntitiesgroupsPost(
        this HttpClient httpClient,
        (string Params, CreateEntitiesGroupPayload Body) param,
        CancellationToken ct = default
    ) => _createEntitiesGroupKbKbidEntitiesgroupsPost(httpClient, param, ct);
    
    /// <summary>Get Knowledge Box Entities</summary>
    public static Task<Result<KnowledgeBoxEntities, HttpError<string>>> GetEntitiesKbKbidEntitiesgroupsGet(
        this HttpClient httpClient,
        string kbid, bool showEntities,
        CancellationToken ct = default
    ) => _getEntitiesKbKbidEntitiesgroupsGet(httpClient, (kbid, showEntities), ct);
    
    /// <summary>Start an export of a Knowledge Box</summary>
    public static Task<Result<CreateExportResponse, HttpError<string>>> StartKbExportEndpointKbKbidExportPost(
        this HttpClient httpClient,
        (string Params, object Body) param,
        CancellationToken ct = default
    ) => _startKbExportEndpointKbKbidExportPost(httpClient, param, ct);
    
    /// <summary>Download a Knowledge Box export</summary>
    public static Task<Result<object, HttpError<string>>> DownloadExportKbEndpointKbKbidExportExportIdGet(
        this HttpClient httpClient,
        string kbid, string exportId,
        CancellationToken ct = default
    ) => _downloadExportKbEndpointKbKbidExportExportIdGet(httpClient, (kbid, exportId), ct);
    
    /// <summary>Get the status of a Knowledge Box Export</summary>
    public static Task<Result<StatusResponse, HttpError<string>>> GetExportStatusEndpointKbKbidExportExportIdStatusGet(
        this HttpClient httpClient,
        string kbid, string exportId,
        CancellationToken ct = default
    ) => _getExportStatusEndpointKbKbidExportExportIdStatusGet(httpClient, (kbid, exportId), ct);
    
    /// <summary>Add a extract strategy to a KB</summary>
    public static Task<Result<string, HttpError<string>>> AddStrategyKbKbidExtractStrategiesPost(
        this HttpClient httpClient,
        (string Params, ExtractConfig Body) param,
        CancellationToken ct = default
    ) => _addStrategyKbKbidExtractStrategiesPost(httpClient, param, ct);
    
    /// <summary>Learning extract strategies</summary>
    public static Task<Result<object, HttpError<string>>> GetExtractStrategiesKbKbidExtractStrategiesGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getExtractStrategiesKbKbidExtractStrategiesGet(httpClient, kbid, ct);
    
    /// <summary>Remove a extract strategy from a KB</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteStrategyKbKbidExtractStrategiesStrategyStrategyIdDelete(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken ct = default
    ) => _deleteStrategyKbKbidExtractStrategiesStrategyStrategyIdDelete(httpClient, (kbid, strategyId), ct);
    
    /// <summary>Extract strategy configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken ct = default
    ) => _getExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet(httpClient, (kbid, strategyId), ct);
    
    /// <summary>Send Feedback</summary>
    public static Task<Result<object, HttpError<string>>> SendFeedbackEndpointKbKbidFeedbackPost(
        this HttpClient httpClient,
        (string Params, FeedbackRequest Body) param,
        CancellationToken ct = default
    ) => _sendFeedbackEndpointKbKbidFeedbackPost(httpClient, param, ct);
    
    /// <summary>Find Knowledge Box</summary>
    public static Task<Result<KnowledgeboxFindResults, HttpError<string>>> FindKnowledgeboxKbKbidFindGet(
        this HttpClient httpClient,
        string kbid, string query, object filterExpression, List<string> fields, List<string> filters, object topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<FindOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, RankFusionName rankFusion, object reranker, object searchConfiguration, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor,
        CancellationToken ct = default
    ) => _findKnowledgeboxKbKbidFindGet(httpClient, (kbid, query, filterExpression, fields, filters, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, rankFusion, reranker, searchConfiguration, xNdbClient, xNucliadbUser, xForwardedFor), ct);
    
    /// <summary>Find Knowledge Box</summary>
    public static Task<Result<KnowledgeboxFindResults, HttpError<string>>> FindPostKnowledgeboxKbKbidFindPost(
        this HttpClient httpClient,
        (string Params, FindRequest Body) param,
        CancellationToken ct = default
    ) => _findPostKnowledgeboxKbKbidFindPost(httpClient, param, ct);
    
    /// <summary>Search Knowledge Box graph</summary>
    public static Task<Result<GraphSearchResponse, HttpError<string>>> GraphSearchKnowledgeboxKbKbidGraphPost(
        this HttpClient httpClient,
        (string Params, GraphSearchRequest Body) param,
        CancellationToken ct = default
    ) => _graphSearchKnowledgeboxKbKbidGraphPost(httpClient, param, ct);
    
    /// <summary>Search Knowledge Box graph nodes</summary>
    public static Task<Result<GraphNodesSearchResponse, HttpError<string>>> GraphNodesSearchKnowledgeboxKbKbidGraphNodesPost(
        this HttpClient httpClient,
        (string Params, GraphNodesSearchRequest Body) param,
        CancellationToken ct = default
    ) => _graphNodesSearchKnowledgeboxKbKbidGraphNodesPost(httpClient, param, ct);
    
    /// <summary>Search Knowledge Box graph relations</summary>
    public static Task<Result<GraphRelationsSearchResponse, HttpError<string>>> GraphRelationsSearchKnowledgeboxKbKbidGraphRelationsPost(
        this HttpClient httpClient,
        (string Params, GraphRelationsSearchRequest Body) param,
        CancellationToken ct = default
    ) => _graphRelationsSearchKnowledgeboxKbKbidGraphRelationsPost(httpClient, param, ct);
    
    /// <summary>Start an import to a Knowledge Box</summary>
    public static Task<Result<CreateImportResponse, HttpError<string>>> StartKbImportEndpointKbKbidImportPost(
        this HttpClient httpClient,
        (string Params, object Body) param,
        CancellationToken ct = default
    ) => _startKbImportEndpointKbKbidImportPost(httpClient, param, ct);
    
    /// <summary>Get the status of a Knowledge Box Import</summary>
    public static Task<Result<StatusResponse, HttpError<string>>> GetImportStatusEndpointKbKbidImportImportIdStatusGet(
        this HttpClient httpClient,
        string kbid, string importId,
        CancellationToken ct = default
    ) => _getImportStatusEndpointKbKbidImportImportIdStatusGet(httpClient, (kbid, importId), ct);
    
    /// <summary>Set Knowledge Box Labels</summary>
    public static Task<Result<object, HttpError<string>>> SetLabelsetEndpointKbKbidLabelsetLabelsetPost(
        this HttpClient httpClient,
        ((string kbid, string labelset) Params, LabelSet Body) param,
        CancellationToken ct = default
    ) => _setLabelsetEndpointKbKbidLabelsetLabelsetPost(httpClient, param, ct);
    
    /// <summary>Delete Knowledge Box Label</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteLabelsetEndpointKbKbidLabelsetLabelsetDelete(
        this HttpClient httpClient,
        string kbid, string labelset,
        CancellationToken ct = default
    ) => _deleteLabelsetEndpointKbKbidLabelsetLabelsetDelete(httpClient, (kbid, labelset), ct);
    
    /// <summary>Get a Knowledge Box Label Set</summary>
    public static Task<Result<LabelSet, HttpError<string>>> GetLabelsetEndpointKbKbidLabelsetLabelsetGet(
        this HttpClient httpClient,
        string kbid, string labelset,
        CancellationToken ct = default
    ) => _getLabelsetEndpointKbKbidLabelsetLabelsetGet(httpClient, (kbid, labelset), ct);
    
    /// <summary>Get Knowledge Box Label Sets</summary>
    public static Task<Result<KnowledgeBoxLabels, HttpError<string>>> GetLabelsetsEndointKbKbidLabelsetsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getLabelsetsEndointKbKbidLabelsetsGet(httpClient, kbid, ct);
    
    /// <summary>Get model metadata</summary>
    public static Task<Result<object, HttpError<string>>> GetModelKbKbidModelModelIdGet(
        this HttpClient httpClient,
        string kbid, string modelId,
        CancellationToken ct = default
    ) => _getModelKbKbidModelModelIdGet(httpClient, (kbid, modelId), ct);
    
    /// <summary>Get available models</summary>
    public static Task<Result<object, HttpError<string>>> GetModelsKbKbidModelsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getModelsKbKbidModelsGet(httpClient, kbid, ct);
    
    /// <summary>Download the Knowledege Box model</summary>
    public static Task<Result<object, HttpError<string>>> DownloadModelKbKbidModelsModelIdFilenameGet(
        this HttpClient httpClient,
        string kbid, string modelId, string filename,
        CancellationToken ct = default
    ) => _downloadModelKbKbidModelsModelIdFilenameGet(httpClient, (kbid, modelId, filename), ct);
    
    /// <summary>Knowledge Box Notifications Stream</summary>
    public static Task<Result<object, HttpError<string>>> NotificationsEndpointKbKbidNotificationsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _notificationsEndpointKbKbidNotificationsGet(httpClient, kbid, ct);
    
    /// <summary>Predict API Proxy</summary>
    public static Task<Result<object, HttpError<string>>> PredictProxyEndpointKbKbidPredictEndpointPost(
        this HttpClient httpClient,
        ((string kbid, PredictProxiedEndpoints endpoint) Params, object Body) param,
        CancellationToken ct = default
    ) => _predictProxyEndpointKbKbidPredictEndpointPost(httpClient, param, ct);
    
    /// <summary>Predict API Proxy</summary>
    public static Task<Result<object, HttpError<string>>> PredictProxyEndpointKbKbidPredictEndpointGet(
        this HttpClient httpClient,
        string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor,
        CancellationToken ct = default
    ) => _predictProxyEndpointKbKbidPredictEndpointGet(httpClient, (kbid, endpoint, xNucliadbUser, xNdbClient, xForwardedFor), ct);
    
    /// <summary>Knowledge Box Processing Status</summary>
    public static Task<Result<RequestsResults, HttpError<string>>> ProcessingStatusKbKbidProcessingStatusGet(
        this HttpClient httpClient,
        string kbid, object cursor, object scheduled, int limit,
        CancellationToken ct = default
    ) => _processingStatusKbKbidProcessingStatusGet(httpClient, (kbid, cursor, scheduled, limit), ct);
    
    /// <summary>Create new upload on a Resource (by id)</summary>
    public static Task<Result<object, HttpError<string>>> TusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadPost(
        this HttpClient httpClient,
        ((string kbid, string pathRid, string field) Params, object Body) param,
        CancellationToken ct = default
    ) => _tusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadPost(httpClient, param, ct);
    
    /// <summary>Upload information</summary>
    public static Task<Result<object, HttpError<string>>> UploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdHead(
        this HttpClient httpClient,
        string kbid, string pathRid, string field, string uploadId,
        CancellationToken ct = default
    ) => _uploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdHead(httpClient, (kbid, pathRid, field, uploadId), ct);
    
    /// <summary>Upload binary file on a Resource (by id)</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<string>>> UploadRidPrefixKbKbidResourcePathRidFileFieldUploadPost(
        this HttpClient httpClient,
        ((string kbid, string pathRid, string field) Params, object Body) param,
        CancellationToken ct = default
    ) => _uploadRidPrefixKbKbidResourcePathRidFileFieldUploadPost(httpClient, param, ct);
    
    /// <summary>Modify Resource (by id)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ModifyResourceRidPrefixKbKbidResourceRidPatch(
        this HttpClient httpClient,
        ((string kbid, string rid) Params, UpdateResourcePayload Body) param,
        CancellationToken ct = default
    ) => _modifyResourceRidPrefixKbKbidResourceRidPatch(httpClient, param, ct);
    
    /// <summary>Delete Resource (by id)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceRidPrefixKbKbidResourceRidDelete(
        this HttpClient httpClient,
        string kbid, string rid,
        CancellationToken ct = default
    ) => _deleteResourceRidPrefixKbKbidResourceRidDelete(httpClient, (kbid, rid), ct);
    
    /// <summary>Get Resource (by id)</summary>
    public static Task<Result<NucliadbModelsResourceResource, HttpError<string>>> GetResourceByUuidKbKbidResourceRidGet(
        this HttpClient httpClient,
        string kbid, string rid, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor,
        CancellationToken ct = default
    ) => _getResourceByUuidKbKbidResourceRidGet(httpClient, (kbid, rid, show, fieldType, extracted, xNucliadbUser, xForwardedFor), ct);
    
    /// <summary>Ask a resource (by id)</summary>
    public static Task<Result<SyncAskResponse, HttpError<string>>> ResourceAskEndpointByUuidKbKbidResourceRidAskPost(
        this HttpClient httpClient,
        ((string kbid, string rid) Params, AskRequest Body) param,
        CancellationToken ct = default
    ) => _resourceAskEndpointByUuidKbKbidResourceRidAskPost(httpClient, param, ct);
    
    /// <summary>Add resource conversation field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rid, string fieldId) Params, InputConversationField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdPut(httpClient, param, ct);
    
    /// <summary>Download conversation binary field (by id)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumGet(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, string messageId, int fileNum,
        CancellationToken ct = default
    ) => _downloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumGet(httpClient, (kbid, rid, fieldId, messageId, fileNum), ct);
    
    /// <summary>Append messages to conversation field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AppendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesPut(
        this HttpClient httpClient,
        ((string kbid, string rid, string fieldId) Params, object Body) param,
        CancellationToken ct = default
    ) => _appendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesPut(httpClient, param, ct);
    
    /// <summary>Add resource file field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rid, string fieldId) Params, FileField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdPut(httpClient, param, ct);
    
    /// <summary>Download field binary field (by id)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rid, string fieldId, bool inline,
        CancellationToken ct = default
    ) => _downloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldGet(httpClient, (kbid, rid, fieldId, inline), ct);
    
    /// <summary>Reprocess file field (by id)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ReprocessFileFieldKbKbidResourceRidFileFieldIdReprocessPost(
        this HttpClient httpClient,
        ((string kbid, string rid, string fieldId) Params, object Body) param,
        CancellationToken ct = default
    ) => _reprocessFileFieldKbKbidResourceRidFileFieldIdReprocessPost(httpClient, param, ct);
    
    /// <summary>Upload data on a Resource (by id)</summary>
    public static Task<Result<object, HttpError<string>>> TusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdPatch(
        this HttpClient httpClient,
        ((string kbid, string rid, string field, string uploadId) Params, object Body) param,
        CancellationToken ct = default
    ) => _tusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdPatch(httpClient, param, ct);
    
    /// <summary>Add resource link field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rid, string fieldId) Params, LinkField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdPut(httpClient, param, ct);
    
    /// <summary>Reindex Resource (by id)</summary>
    public static Task<Result<object, HttpError<string>>> ReindexResourceRidPrefixKbKbidResourceRidReindexPost(
        this HttpClient httpClient,
        ((string kbid, string rid) Params, object Body) param,
        CancellationToken ct = default
    ) => _reindexResourceRidPrefixKbKbidResourceRidReindexPost(httpClient, param, ct);
    
    /// <summary>Reprocess resource (by id)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ReprocessResourceRidPrefixKbKbidResourceRidReprocessPost(
        this HttpClient httpClient,
        ((string kbid, string rid) Params, object Body) param,
        CancellationToken ct = default
    ) => _reprocessResourceRidPrefixKbKbidResourceRidReprocessPost(httpClient, param, ct);
    
    /// <summary>Run Agents on Resource</summary>
    public static Task<Result<ResourceAgentsResponse, HttpError<string>>> RunAgentsByUuidKbKbidResourceRidRunAgentsPost(
        this HttpClient httpClient,
        ((string kbid, string rid) Params, ResourceAgentsRequest Body) param,
        CancellationToken ct = default
    ) => _runAgentsByUuidKbKbidResourceRidRunAgentsPost(httpClient, param, ct);
    
    /// <summary>Search on Resource</summary>
    public static Task<Result<ResourceSearchResults, HttpError<string>>> ResourceSearchKbKbidResourceRidSearchGet(
        this HttpClient httpClient,
        string kbid, string rid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, object sortField, SortOrder sortOrder, object topK, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, bool highlight, bool debug, NucliaDBClientType xNdbClient,
        CancellationToken ct = default
    ) => _resourceSearchKbKbidResourceRidSearchGet(httpClient, (kbid, rid, query, filterExpression, fields, filters, faceted, sortField, sortOrder, topK, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, highlight, debug, xNdbClient), ct);
    
    /// <summary>Add resource text field (by id)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rid, string fieldId) Params, TextField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut(httpClient, param, ct);
    
    /// <summary>Delete Resource field (by id)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete(
        this HttpClient httpClient,
        string kbid, string rid, FieldTypeName fieldType, string fieldId,
        CancellationToken ct = default
    ) => _deleteResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete(httpClient, (kbid, rid, fieldType, fieldId), ct);
    
    /// <summary>Get Resource field (by id)</summary>
    public static Task<Result<ResourceField, HttpError<string>>> GetResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet(
        this HttpClient httpClient,
        string kbid, string rid, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page,
        CancellationToken ct = default
    ) => _getResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet(httpClient, (kbid, rid, fieldType, fieldId, show, extracted, page), ct);
    
    /// <summary>Download extracted binary file (by id)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rid, FieldTypeName fieldType, string fieldId, string downloadField,
        CancellationToken ct = default
    ) => _downloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldGet(httpClient, (kbid, rid, fieldType, fieldId, downloadField), ct);
    
    /// <summary>Create Resource</summary>
    public static Task<Result<ResourceCreated, HttpError<string>>> CreateResourceKbKbidResourcesPost(
        this HttpClient httpClient,
        (string Params, CreateResourcePayload Body) param,
        CancellationToken ct = default
    ) => _createResourceKbKbidResourcesPost(httpClient, param, ct);
    
    /// <summary>List Resources</summary>
    public static Task<Result<ResourceList, HttpError<string>>> ListResourcesKbKbidResourcesGet(
        this HttpClient httpClient,
        string kbid, int page, int size,
        CancellationToken ct = default
    ) => _listResourcesKbKbidResourcesGet(httpClient, (kbid, page, size), ct);
    
    /// <summary>Learning configuration schema</summary>
    public static Task<Result<object, HttpError<string>>> GetSchemaForConfigurationUpdatesKbKbidSchemaGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getSchemaForConfigurationUpdatesKbKbidSchemaGet(httpClient, kbid, ct);
    
    /// <summary>Search Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> SearchKnowledgeboxKbKbidSearchGet(
        this HttpClient httpClient,
        string kbid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SearchOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor,
        CancellationToken ct = default
    ) => _searchKnowledgeboxKbKbidSearchGet(httpClient, (kbid, query, filterExpression, fields, filters, faceted, sortField, sortLimit, sortOrder, topK, minScore, minScoreSemantic, minScoreBm25, vectorset, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, debug, highlight, show, fieldType, extracted, withDuplicates, withSynonyms, autofilter, securityGroups, showHidden, xNdbClient, xNucliadbUser, xForwardedFor), ct);
    
    /// <summary>Search Knowledge Box</summary>
    public static Task<Result<KnowledgeboxSearchResults, HttpError<string>>> SearchPostKnowledgeboxKbKbidSearchPost(
        this HttpClient httpClient,
        (string Params, SearchRequest Body) param,
        CancellationToken ct = default
    ) => _searchPostKnowledgeboxKbKbidSearchPost(httpClient, param, ct);
    
    /// <summary>List search configurations</summary>
    public static Task<Result<object, HttpError<string>>> ListSearchConfigurationsKbKbidSearchConfigurationsGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _listSearchConfigurationsKbKbidSearchConfigurationsGet(httpClient, kbid, ct);
    
    /// <summary>Create search configuration</summary>
    public static Task<Result<object, HttpError<string>>> CreateSearchConfigurationKbKbidSearchConfigurationsConfigNamePost(
        this HttpClient httpClient,
        ((string kbid, string configName) Params, object Body) param,
        CancellationToken ct = default
    ) => _createSearchConfigurationKbKbidSearchConfigurationsConfigNamePost(httpClient, param, ct);
    
    /// <summary>Update search configuration</summary>
    public static Task<Result<object, HttpError<string>>> UpdateSearchConfigurationKbKbidSearchConfigurationsConfigNamePatch(
        this HttpClient httpClient,
        ((string kbid, string configName) Params, object Body) param,
        CancellationToken ct = default
    ) => _updateSearchConfigurationKbKbidSearchConfigurationsConfigNamePatch(httpClient, param, ct);
    
    /// <summary>Delete search configuration</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteSearchConfigurationKbKbidSearchConfigurationsConfigNameDelete(
        this HttpClient httpClient,
        string kbid, string configName,
        CancellationToken ct = default
    ) => _deleteSearchConfigurationKbKbidSearchConfigurationsConfigNameDelete(httpClient, (kbid, configName), ct);
    
    /// <summary>Get search configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetSearchConfigurationKbKbidSearchConfigurationsConfigNameGet(
        this HttpClient httpClient,
        string kbid, string configName,
        CancellationToken ct = default
    ) => _getSearchConfigurationKbKbidSearchConfigurationsConfigNameGet(httpClient, (kbid, configName), ct);
    
    /// <summary>Modify Resource (by slug)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ModifyResourceRslugPrefixKbKbidSlugRslugPatch(
        this HttpClient httpClient,
        ((string kbid, string rslug) Params, UpdateResourcePayload Body) param,
        CancellationToken ct = default
    ) => _modifyResourceRslugPrefixKbKbidSlugRslugPatch(httpClient, param, ct);
    
    /// <summary>Delete Resource (by slug)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceRslugPrefixKbKbidSlugRslugDelete(
        this HttpClient httpClient,
        string kbid, string rslug,
        CancellationToken ct = default
    ) => _deleteResourceRslugPrefixKbKbidSlugRslugDelete(httpClient, (kbid, rslug), ct);
    
    /// <summary>Get Resource (by slug)</summary>
    public static Task<Result<NucliadbModelsResourceResource, HttpError<string>>> GetResourceBySlugKbKbidSlugRslugGet(
        this HttpClient httpClient,
        string kbid, string rslug, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor,
        CancellationToken ct = default
    ) => _getResourceBySlugKbKbidSlugRslugGet(httpClient, (kbid, rslug, show, fieldType, extracted, xNucliadbUser, xForwardedFor), ct);
    
    /// <summary>Add resource conversation field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rslug, string fieldId) Params, InputConversationField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdPut(httpClient, param, ct);
    
    /// <summary>Download conversation binary field (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumGet(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, string messageId, int fileNum,
        CancellationToken ct = default
    ) => _downloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumGet(httpClient, (kbid, rslug, fieldId, messageId, fileNum), ct);
    
    /// <summary>Append messages to conversation field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AppendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesPut(
        this HttpClient httpClient,
        ((string kbid, string rslug, string fieldId) Params, object Body) param,
        CancellationToken ct = default
    ) => _appendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesPut(httpClient, param, ct);
    
    /// <summary>Add resource file field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rslug, string fieldId) Params, FileField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdPut(httpClient, param, ct);
    
    /// <summary>Download field binary field (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rslug, string fieldId, bool inline,
        CancellationToken ct = default
    ) => _downloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldGet(httpClient, (kbid, rslug, fieldId, inline), ct);
    
    /// <summary>Create new upload on a Resource (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> TusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadPost(
        this HttpClient httpClient,
        ((string kbid, string rslug, string field) Params, object Body) param,
        CancellationToken ct = default
    ) => _tusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadPost(httpClient, param, ct);
    
    /// <summary>Upload data on a Resource (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> TusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdPatch(
        this HttpClient httpClient,
        ((string kbid, string rslug, string field, string uploadId) Params, object Body) param,
        CancellationToken ct = default
    ) => _tusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdPatch(httpClient, param, ct);
    
    /// <summary>Upload information</summary>
    public static Task<Result<object, HttpError<string>>> UploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdHead(
        this HttpClient httpClient,
        string kbid, string rslug, string field, string uploadId,
        CancellationToken ct = default
    ) => _uploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdHead(httpClient, (kbid, rslug, field, uploadId), ct);
    
    /// <summary>Upload binary file on a Resource (by slug)</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<string>>> UploadRslugPrefixKbKbidSlugRslugFileFieldUploadPost(
        this HttpClient httpClient,
        ((string kbid, string rslug, string field) Params, object Body) param,
        CancellationToken ct = default
    ) => _uploadRslugPrefixKbKbidSlugRslugFileFieldUploadPost(httpClient, param, ct);
    
    /// <summary>Add resource link field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rslug, string fieldId) Params, LinkField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdPut(httpClient, param, ct);
    
    /// <summary>Reindex Resource (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> ReindexResourceRslugPrefixKbKbidSlugRslugReindexPost(
        this HttpClient httpClient,
        ((string kbid, string rslug) Params, object Body) param,
        CancellationToken ct = default
    ) => _reindexResourceRslugPrefixKbKbidSlugRslugReindexPost(httpClient, param, ct);
    
    /// <summary>Reprocess resource (by slug)</summary>
    public static Task<Result<ResourceUpdated, HttpError<string>>> ReprocessResourceRslugPrefixKbKbidSlugRslugReprocessPost(
        this HttpClient httpClient,
        ((string kbid, string rslug) Params, object Body) param,
        CancellationToken ct = default
    ) => _reprocessResourceRslugPrefixKbKbidSlugRslugReprocessPost(httpClient, param, ct);
    
    /// <summary>Add resource text field (by slug)</summary>
    public static Task<Result<ResourceFieldAdded, HttpError<string>>> AddResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdPut(
        this HttpClient httpClient,
        ((string kbid, string rslug, string fieldId) Params, TextField Body) param,
        CancellationToken ct = default
    ) => _addResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdPut(httpClient, param, ct);
    
    /// <summary>Delete Resource field (by slug)</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete(
        this HttpClient httpClient,
        string kbid, string rslug, FieldTypeName fieldType, string fieldId,
        CancellationToken ct = default
    ) => _deleteResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete(httpClient, (kbid, rslug, fieldType, fieldId), ct);
    
    /// <summary>Get Resource field (by slug)</summary>
    public static Task<Result<ResourceField, HttpError<string>>> GetResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet(
        this HttpClient httpClient,
        string kbid, string rslug, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page,
        CancellationToken ct = default
    ) => _getResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet(httpClient, (kbid, rslug, fieldType, fieldId, show, extracted, page), ct);
    
    /// <summary>Download extracted binary file (by slug)</summary>
    public static Task<Result<object, HttpError<string>>> DownloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldGet(
        this HttpClient httpClient,
        string kbid, string rslug, FieldTypeName fieldType, string fieldId, string downloadField,
        CancellationToken ct = default
    ) => _downloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldGet(httpClient, (kbid, rslug, fieldType, fieldId, downloadField), ct);
    
    /// <summary>Ask a resource (by slug)</summary>
    public static Task<Result<SyncAskResponse, HttpError<string>>> ResourceAskEndpointBySlugKbKbidSlugSlugAskPost(
        this HttpClient httpClient,
        ((string kbid, string slug) Params, AskRequest Body) param,
        CancellationToken ct = default
    ) => _resourceAskEndpointBySlugKbKbidSlugSlugAskPost(httpClient, param, ct);
    
    /// <summary>Run Agents on Resource (by slug)</summary>
    public static Task<Result<ResourceAgentsResponse, HttpError<string>>> RunAgentsBySlugKbKbidSlugSlugRunAgentsPost(
        this HttpClient httpClient,
        ((string kbid, string slug) Params, ResourceAgentsRequest Body) param,
        CancellationToken ct = default
    ) => _runAgentsBySlugKbKbidSlugSlugRunAgentsPost(httpClient, param, ct);
    
    /// <summary>Add a split strategy to a KB</summary>
    public static Task<Result<string, HttpError<string>>> AddSplitStrategyKbKbidSplitStrategiesPost(
        this HttpClient httpClient,
        (string Params, SplitConfiguration Body) param,
        CancellationToken ct = default
    ) => _addSplitStrategyKbKbidSplitStrategiesPost(httpClient, param, ct);
    
    /// <summary>Learning split strategies</summary>
    public static Task<Result<object, HttpError<string>>> GetSplitStrategiesKbKbidSplitStrategiesGet(
        this HttpClient httpClient,
        string kbid,
        CancellationToken ct = default
    ) => _getSplitStrategiesKbKbidSplitStrategiesGet(httpClient, kbid, ct);
    
    /// <summary>Remove a split strategy from a KB</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteSplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken ct = default
    ) => _deleteSplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete(httpClient, (kbid, strategyId), ct);
    
    /// <summary>Extract split configuration</summary>
    public static Task<Result<object, HttpError<string>>> GetSplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet(
        this HttpClient httpClient,
        string kbid, string strategyId,
        CancellationToken ct = default
    ) => _getSplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet(httpClient, (kbid, strategyId), ct);
    
    /// <summary>Suggest on a knowledge box</summary>
    public static Task<Result<KnowledgeboxSuggestResults, HttpError<string>>> SuggestKnowledgeboxKbKbidSuggestGet(
        this HttpClient httpClient,
        string kbid, string query, List<string> fields, List<string> filters, List<string> faceted, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SuggestOptions> features, List<ResourceProperties> show, List<FieldTypeName> fieldType, bool debug, bool highlight, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor,
        CancellationToken ct = default
    ) => _suggestKnowledgeboxKbKbidSuggestGet(httpClient, (kbid, query, fields, filters, faceted, rangeCreationStart, rangeCreationEnd, rangeModificationStart, rangeModificationEnd, features, show, fieldType, debug, highlight, showHidden, xNdbClient, xNucliadbUser, xForwardedFor), ct);
    
    /// <summary>Summarize your documents</summary>
    public static Task<Result<SummarizedResponse, HttpError<string>>> SummarizeEndpointKbKbidSummarizePost(
        this HttpClient httpClient,
        (string Params, SummarizeRequest Body) param,
        CancellationToken ct = default
    ) => _summarizeEndpointKbKbidSummarizePost(httpClient, param, ct);
    
    /// <summary>Create new upload on a Knowledge Box</summary>
    public static Task<Result<object, HttpError<string>>> TusPostKbKbidTusuploadPost(
        this HttpClient httpClient,
        (string Params, object Body) param,
        CancellationToken ct = default
    ) => _tusPostKbKbidTusuploadPost(httpClient, param, ct);
    
    /// <summary>TUS Server information</summary>
    public static Task<Result<object, HttpError<string>>> TusOptionsKbKbidTusuploadOptions(
        this HttpClient httpClient,
        string kbid, object rid, object rslug, object uploadId, object field,
        CancellationToken ct = default
    ) => _tusOptionsKbKbidTusuploadOptions(httpClient, (kbid, rid, rslug, uploadId, field), ct);
    
    /// <summary>Upload data on a Knowledge Box</summary>
    public static Task<Result<object, HttpError<string>>> PatchKbKbidTusuploadUploadIdPatch(
        this HttpClient httpClient,
        ((string kbid, string uploadId) Params, object Body) param,
        CancellationToken ct = default
    ) => _patchKbKbidTusuploadUploadIdPatch(httpClient, param, ct);
    
    /// <summary>Upload information</summary>
    public static Task<Result<object, HttpError<string>>> UploadInformationKbKbidTusuploadUploadIdHead(
        this HttpClient httpClient,
        string kbid, string uploadId,
        CancellationToken ct = default
    ) => _uploadInformationKbKbidTusuploadUploadIdHead(httpClient, (kbid, uploadId), ct);
    
    /// <summary>Upload binary file on a Knowledge Box</summary>
    public static Task<Result<ResourceFileUploaded, HttpError<string>>> UploadKbKbidUploadPost(
        this HttpClient httpClient,
        (string Params, object Body) param,
        CancellationToken ct = default
    ) => _uploadKbKbidUploadPost(httpClient, param, ct);

    #endregion

    #region Learning Operations

    /// <summary>Learning Configuration Schema</summary>
    public static Task<Result<object, HttpError<string>>> LearningConfigurationSchemaLearningConfigurationSchemaGet(
        this HttpClient httpClient,
        CancellationToken ct = default
    ) => _learningConfigurationSchemaLearningConfigurationSchemaGet(httpClient, Unit.Value, ct);

    #endregion

    private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
        Task.FromResult(Unit.Value);

    #region Kb Operations

    private static GetAsync<KnowledgeBoxObj, string, string> _getKbBySlugKbSSlugGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxObj, string, string>(
            url: BaseUrl,
            buildRequest: static slug => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/s/{slug}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeBoxObj, string, string> _getKbKbKbidGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxObj, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxObj>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<SyncAskResponse, string, (string Params, AskRequest Body)> _askKnowledgeboxEndpointKbKbidAskPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, string, (string Params, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/ask"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int pageNumber, int pageSize, object withStatus, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, object hidden, List<ResourceProperties> show)> _catalogGetKbKbidCatalogGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int pageNumber, int pageSize, object withStatus, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, object hidden, List<ResourceProperties> show)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/catalog?query={param.query}&filter_expression={param.filterExpression}&filters={param.filters}&faceted={param.faceted}&sort_field={param.sortField}&sort_limit={param.sortLimit}&sort_order={param.sortOrder}&page_number={param.pageNumber}&page_size={param.pageSize}&with_status={param.withStatus}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&hidden={param.hidden}&show={param.show}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<KnowledgeboxSearchResults, string, (string Params, CatalogRequest Body)> _catalogPostKbKbidCatalogPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxSearchResults, string, (string Params, CatalogRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/catalog"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _getConfigurationKbKbidConfigurationGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/configuration"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<object, string, (string Params, object Body)> _patchConfigurationKbKbidConfigurationPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/configuration"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, (string Params, object Body)> _setConfigurationKbKbidConfigurationPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/configuration"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeboxCounters, string, (string kbid, bool debug)> _knowledgeboxCountersKbKbidCountersGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxCounters, string, (string kbid, bool debug)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/counters?debug={param.debug}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxCounters>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<object, string, (string Params, KnowledgeBoxSynonyms Body)> _setCustomSynonymsKbKbidCustomSynonymsPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, string, (string Params, KnowledgeBoxSynonyms Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/custom-synonyms"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, string> _deleteCustomSynonymsKbKbidCustomSynonymsDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/custom-synonyms"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeBoxSynonyms, string, string> _getCustomSynonymsKbKbidCustomSynonymsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxSynonyms, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/custom-synonyms"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxSynonyms>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<object, string, ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body)> _updateEntitiesGroupKbKbidEntitiesgroupGroupPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string group) Params, UpdateEntitiesGroupPayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/entitiesgroup/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string group)> _deleteEntitiesKbKbidEntitiesgroupGroupDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string group)>(
            url: BaseUrl,
            buildRequest: static (kbid, group) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/entitiesgroup/{group}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<EntitiesGroup, string, (string kbid, string group)> _getEntityKbKbidEntitiesgroupGroupGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<EntitiesGroup, string, (string kbid, string group)>(
            url: BaseUrl,
            buildRequest: static (kbid, group) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/entitiesgroup/{group}"), null, null),
            deserializeSuccess: DeserializeJson<EntitiesGroup>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, (string Params, CreateEntitiesGroupPayload Body)> _createEntitiesGroupKbKbidEntitiesgroupsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string Params, CreateEntitiesGroupPayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/entitiesgroups"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeBoxEntities, string, (string kbid, bool showEntities)> _getEntitiesKbKbidEntitiesgroupsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxEntities, string, (string kbid, bool showEntities)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/entitiesgroups?show_entities={param.showEntities}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxEntities>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<CreateExportResponse, string, (string Params, object Body)> _startKbExportEndpointKbKbidExportPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateExportResponse, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/export"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<CreateExportResponse>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string exportId)> _downloadExportKbEndpointKbKbidExportExportIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string exportId)>(
            url: BaseUrl,
            buildRequest: static (kbid, exportId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/export/{export_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<StatusResponse, string, (string kbid, string exportId)> _getExportStatusEndpointKbKbidExportExportIdStatusGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<StatusResponse, string, (string kbid, string exportId)>(
            url: BaseUrl,
            buildRequest: static (kbid, exportId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/export/{export_id}/status"), null, null),
            deserializeSuccess: DeserializeJson<StatusResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<string, string, (string Params, ExtractConfig Body)> _addStrategyKbKbidExtractStrategiesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<string, string, (string Params, ExtractConfig Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/extract_strategies"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<string>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _getExtractStrategiesKbKbidExtractStrategiesGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/extract_strategies"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string strategyId)> _deleteStrategyKbKbidExtractStrategiesStrategyStrategyIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static (kbid, strategyId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/extract_strategies/strategy/{strategy_id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string strategyId)> _getExtractStrategyFromIdKbKbidExtractStrategiesStrategyStrategyIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static (kbid, strategyId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/extract_strategies/strategy/{strategy_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, (string Params, FeedbackRequest Body)> _sendFeedbackEndpointKbKbidFeedbackPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string Params, FeedbackRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/feedback"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeboxFindResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, object topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<FindOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, RankFusionName rankFusion, object reranker, object searchConfiguration, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)> _findKnowledgeboxKbKbidFindGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxFindResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, object topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<FindOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, RankFusionName rankFusion, object reranker, object searchConfiguration, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/find?query={param.query}&filter_expression={param.filterExpression}&fields={param.fields}&filters={param.filters}&top_k={param.topK}&min_score={param.minScore}&min_score_semantic={param.minScoreSemantic}&min_score_bm25={param.minScoreBm25}&vectorset={param.vectorset}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&features={param.features}&debug={param.debug}&highlight={param.highlight}&show={param.show}&field_type={param.fieldType}&extracted={param.extracted}&with_duplicates={param.withDuplicates}&with_synonyms={param.withSynonyms}&autofilter={param.autofilter}&security_groups={param.securityGroups}&show_hidden={param.showHidden}&rank_fusion={param.rankFusion}&reranker={param.reranker}&search_configuration={param.searchConfiguration}&x-ndb-client={param.xNdbClient}&x-nucliadb-user={param.xNucliadbUser}&x-forwarded-for={param.xForwardedFor}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxFindResults>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<KnowledgeboxFindResults, string, (string Params, FindRequest Body)> _findPostKnowledgeboxKbKbidFindPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxFindResults, string, (string Params, FindRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/find"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<KnowledgeboxFindResults>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<GraphSearchResponse, string, (string Params, GraphSearchRequest Body)> _graphSearchKnowledgeboxKbKbidGraphPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphSearchResponse, string, (string Params, GraphSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/graph"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<GraphSearchResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<GraphNodesSearchResponse, string, (string Params, GraphNodesSearchRequest Body)> _graphNodesSearchKnowledgeboxKbKbidGraphNodesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphNodesSearchResponse, string, (string Params, GraphNodesSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/graph/nodes"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<GraphNodesSearchResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<GraphRelationsSearchResponse, string, (string Params, GraphRelationsSearchRequest Body)> _graphRelationsSearchKnowledgeboxKbKbidGraphRelationsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GraphRelationsSearchResponse, string, (string Params, GraphRelationsSearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/graph/relations"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<GraphRelationsSearchResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<CreateImportResponse, string, (string Params, object Body)> _startKbImportEndpointKbKbidImportPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateImportResponse, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/import"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<CreateImportResponse>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<StatusResponse, string, (string kbid, string importId)> _getImportStatusEndpointKbKbidImportImportIdStatusGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<StatusResponse, string, (string kbid, string importId)>(
            url: BaseUrl,
            buildRequest: static (kbid, importId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/import/{import_id}/status"), null, null),
            deserializeSuccess: DeserializeJson<StatusResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, string labelset) Params, LabelSet Body)> _setLabelsetEndpointKbKbidLabelsetLabelsetPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string labelset) Params, LabelSet Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/labelset/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string labelset)> _deleteLabelsetEndpointKbKbidLabelsetLabelsetDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string labelset)>(
            url: BaseUrl,
            buildRequest: static (kbid, labelset) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/labelset/{labelset}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<LabelSet, string, (string kbid, string labelset)> _getLabelsetEndpointKbKbidLabelsetLabelsetGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<LabelSet, string, (string kbid, string labelset)>(
            url: BaseUrl,
            buildRequest: static (kbid, labelset) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/labelset/{labelset}"), null, null),
            deserializeSuccess: DeserializeJson<LabelSet>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeBoxLabels, string, string> _getLabelsetsEndointKbKbidLabelsetsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeBoxLabels, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/labelsets"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeBoxLabels>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string modelId)> _getModelKbKbidModelModelIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string modelId)>(
            url: BaseUrl,
            buildRequest: static (kbid, modelId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/model/{model_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _getModelsKbKbidModelsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/models"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string modelId, string filename)> _downloadModelKbKbidModelsModelIdFilenameGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string modelId, string filename)>(
            url: BaseUrl,
            buildRequest: static (kbid, modelId, filename) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/models/{model_id}/{filename}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _notificationsEndpointKbKbidNotificationsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/notifications"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, PredictProxiedEndpoints endpoint) Params, object Body)> _predictProxyEndpointKbKbidPredictEndpointPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, PredictProxiedEndpoints endpoint) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/predict/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor)> _predictProxyEndpointKbKbidPredictEndpointGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, PredictProxiedEndpoints endpoint, string xNucliadbUser, NucliaDBClientType xNdbClient, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/predict/{param.endpoint}?x-nucliadb-user={param.xNucliadbUser}&x-ndb-client={param.xNdbClient}&x-forwarded-for={param.xForwardedFor}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<RequestsResults, string, (string kbid, object cursor, object scheduled, int limit)> _processingStatusKbKbidProcessingStatusGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<RequestsResults, string, (string kbid, object cursor, object scheduled, int limit)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/processing-status?cursor={param.cursor}&scheduled={param.scheduled}&limit={param.limit}"), null, null),
            deserializeSuccess: DeserializeJson<RequestsResults>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, string pathRid, string field) Params, object Body)> _tusPostRidPrefixKbKbidResourcePathRidFileFieldTusuploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string pathRid, string field) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/file/{param.Params}/tusupload"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static HEADAsync<object, string, (string kbid, string pathRid, string field, string uploadId)> _uploadInformationKbKbidResourcePathRidFileFieldTusuploadUploadIdHead { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHEAD<object, string, (string kbid, string pathRid, string field, string uploadId)>(
            url: BaseUrl,
            buildRequest: static (kbid, pathRid, field, uploadId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/resource/{path_rid}/file/{field}/tusupload/{upload_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceFileUploaded, string, ((string kbid, string pathRid, string field) Params, object Body)> _uploadRidPrefixKbKbidResourcePathRidFileFieldUploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, string, ((string kbid, string pathRid, string field) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/file/{param.Params}/upload"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<ResourceUpdated, string, ((string kbid, string rid) Params, UpdateResourcePayload Body)> _modifyResourceRidPrefixKbKbidResourceRidPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<ResourceUpdated, string, ((string kbid, string rid) Params, UpdateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string rid)> _deleteResourceRidPrefixKbKbidResourceRidDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rid)>(
            url: BaseUrl,
            buildRequest: static (kbid, rid) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/resource/{rid}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<NucliadbModelsResourceResource, string, (string kbid, string rid, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)> _getResourceByUuidKbKbidResourceRidGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NucliadbModelsResourceResource, string, (string kbid, string rid, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}?show={param.show}&field_type={param.fieldType}&extracted={param.extracted}&x-nucliadb-user={param.xNucliadbUser}&x-forwarded-for={param.xForwardedFor}"), null, null),
            deserializeSuccess: DeserializeJson<NucliadbModelsResourceResource>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<SyncAskResponse, string, ((string kbid, string rid) Params, AskRequest Body)> _resourceAskEndpointByUuidKbKbidResourceRidAskPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, string, ((string kbid, string rid) Params, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/ask"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, InputConversationField Body)> _addResourceFieldConversationRidPrefixKbKbidResourceRidConversationFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, InputConversationField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/conversation/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string rid, string fieldId, string messageId, int fileNum)> _downloadFieldConversationAttachmentRidPrefixKbKbidResourceRidConversationFieldIdDownloadFieldMessageIdFileNumGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rid, string fieldId, string messageId, int fileNum)>(
            url: BaseUrl,
            buildRequest: static (kbid, rid, fieldId, messageId, fileNum) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/resource/{rid}/conversation/{field_id}/download/field/{message_id}/{file_num}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, object Body)> _appendMessagesToConversationFieldRidPrefixKbKbidResourceRidConversationFieldIdMessagesPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/conversation/{param.Params}/messages"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, FileField Body)> _addResourceFieldFileRidPrefixKbKbidResourceRidFileFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, FileField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/file/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string rid, string fieldId, bool inline)> _downloadFieldFileRidPrefixKbKbidResourceRidFileFieldIdDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rid, string fieldId, bool inline)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/file/{param.field_id}/download/field?inline={param.inline}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceUpdated, string, ((string kbid, string rid, string fieldId) Params, object Body)> _reprocessFileFieldKbKbidResourceRidFileFieldIdReprocessPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, string, ((string kbid, string rid, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/file/{param.Params}/reprocess"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<object, string, ((string kbid, string rid, string field, string uploadId) Params, object Body)> _tusPatchRidPrefixKbKbidResourceRidFileFieldTusuploadUploadIdPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string rid, string field, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/file/{param.Params}/tusupload/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, LinkField Body)> _addResourceFieldLinkRidPrefixKbKbidResourceRidLinkFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, LinkField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/link/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, string rid) Params, object Body)> _reindexResourceRidPrefixKbKbidResourceRidReindexPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string rid) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/reindex"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceUpdated, string, ((string kbid, string rid) Params, object Body)> _reprocessResourceRidPrefixKbKbidResourceRidReprocessPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, string, ((string kbid, string rid) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/reprocess"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceAgentsResponse, string, ((string kbid, string rid) Params, ResourceAgentsRequest Body)> _runAgentsByUuidKbKbidResourceRidRunAgentsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceAgentsResponse, string, ((string kbid, string rid) Params, ResourceAgentsRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/run-agents"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceAgentsResponse>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<ResourceSearchResults, string, (string kbid, string rid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, object sortField, SortOrder sortOrder, object topK, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, bool highlight, bool debug, NucliaDBClientType xNdbClient)> _resourceSearchKbKbidResourceRidSearchGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceSearchResults, string, (string kbid, string rid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, object sortField, SortOrder sortOrder, object topK, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, bool highlight, bool debug, NucliaDBClientType xNdbClient)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/search?query={param.query}&filter_expression={param.filterExpression}&fields={param.fields}&filters={param.filters}&faceted={param.faceted}&sort_field={param.sortField}&sort_order={param.sortOrder}&top_k={param.topK}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&highlight={param.highlight}&debug={param.debug}&x-ndb-client={param.xNdbClient}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceSearchResults>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, TextField Body)> _addResourceFieldTextRidPrefixKbKbidResourceRidTextFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rid, string fieldId) Params, TextField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resource/{param.Params}/text/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId)> _deleteResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId)>(
            url: BaseUrl,
            buildRequest: static (kbid, rid, fieldType, fieldId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/resource/{rid}/{field_type}/{field_id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<ResourceField, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)> _getResourceFieldRidPrefixKbKbidResourceRidFieldTypeFieldIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceField, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resource/{param.rid}/{param.field_type}/{param.field_id}?show={param.show}&extracted={param.extracted}&page={param.page}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceField>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, string downloadField)> _downloadExtractFileRidPrefixKbKbidResourceRidFieldTypeFieldIdDownloadExtractedDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rid, FieldTypeName fieldType, string fieldId, string downloadField)>(
            url: BaseUrl,
            buildRequest: static (kbid, rid, fieldType, fieldId, downloadField) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/resource/{rid}/{field_type}/{field_id}/download/extracted/{download_field}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceCreated, string, (string Params, CreateResourcePayload Body)> _createResourceKbKbidResourcesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceCreated, string, (string Params, CreateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/resources"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceCreated>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<ResourceList, string, (string kbid, int page, int size)> _listResourcesKbKbidResourcesGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceList, string, (string kbid, int page, int size)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/resources?page={param.page}&size={param.size}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceList>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _getSchemaForConfigurationUpdatesKbKbidSchemaGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/schema"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SearchOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)> _searchKnowledgeboxKbKbidSearchGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSearchResults, string, (string kbid, string query, object filterExpression, List<string> fields, List<string> filters, List<string> faceted, SortField sortField, object sortLimit, SortOrder sortOrder, int topK, object minScore, object minScoreSemantic, float minScoreBm25, object vectorset, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SearchOptions> features, bool debug, bool highlight, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, bool withDuplicates, bool withSynonyms, bool autofilter, List<string> securityGroups, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/search?query={param.query}&filter_expression={param.filterExpression}&fields={param.fields}&filters={param.filters}&faceted={param.faceted}&sort_field={param.sortField}&sort_limit={param.sortLimit}&sort_order={param.sortOrder}&top_k={param.topK}&min_score={param.minScore}&min_score_semantic={param.minScoreSemantic}&min_score_bm25={param.minScoreBm25}&vectorset={param.vectorset}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&features={param.features}&debug={param.debug}&highlight={param.highlight}&show={param.show}&field_type={param.fieldType}&extracted={param.extracted}&with_duplicates={param.withDuplicates}&with_synonyms={param.withSynonyms}&autofilter={param.autofilter}&security_groups={param.securityGroups}&show_hidden={param.showHidden}&x-ndb-client={param.xNdbClient}&x-nucliadb-user={param.xNucliadbUser}&x-forwarded-for={param.xForwardedFor}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<KnowledgeboxSearchResults, string, (string Params, SearchRequest Body)> _searchPostKnowledgeboxKbKbidSearchPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<KnowledgeboxSearchResults, string, (string Params, SearchRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/search"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSearchResults>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _listSearchConfigurationsKbKbidSearchConfigurationsGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/search_configurations"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, string configName) Params, object Body)> _createSearchConfigurationKbKbidSearchConfigurationsConfigNamePost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string configName) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/search_configurations/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<object, string, ((string kbid, string configName) Params, object Body)> _updateSearchConfigurationKbKbidSearchConfigurationsConfigNamePatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string configName) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/search_configurations/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string configName)> _deleteSearchConfigurationKbKbidSearchConfigurationsConfigNameDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string configName)>(
            url: BaseUrl,
            buildRequest: static (kbid, configName) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/search_configurations/{config_name}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string configName)> _getSearchConfigurationKbKbidSearchConfigurationsConfigNameGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string configName)>(
            url: BaseUrl,
            buildRequest: static (kbid, configName) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/search_configurations/{config_name}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<ResourceUpdated, string, ((string kbid, string rslug) Params, UpdateResourcePayload Body)> _modifyResourceRslugPrefixKbKbidSlugRslugPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<ResourceUpdated, string, ((string kbid, string rslug) Params, UpdateResourcePayload Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string rslug)> _deleteResourceRslugPrefixKbKbidSlugRslugDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rslug)>(
            url: BaseUrl,
            buildRequest: static (kbid, rslug) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/slug/{rslug}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<NucliadbModelsResourceResource, string, (string kbid, string rslug, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)> _getResourceBySlugKbKbidSlugRslugGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NucliadbModelsResourceResource, string, (string kbid, string rslug, List<ResourceProperties> show, List<FieldTypeName> fieldType, List<ExtractedDataTypeName> extracted, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}?show={param.show}&field_type={param.fieldType}&extracted={param.extracted}&x-nucliadb-user={param.xNucliadbUser}&x-forwarded-for={param.xForwardedFor}"), null, null),
            deserializeSuccess: DeserializeJson<NucliadbModelsResourceResource>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, InputConversationField Body)> _addResourceFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, InputConversationField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/conversation/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string rslug, string fieldId, string messageId, int fileNum)> _downloadFieldConversationRslugPrefixKbKbidSlugRslugConversationFieldIdDownloadFieldMessageIdFileNumGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rslug, string fieldId, string messageId, int fileNum)>(
            url: BaseUrl,
            buildRequest: static (kbid, rslug, fieldId, messageId, fileNum) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/slug/{rslug}/conversation/{field_id}/download/field/{message_id}/{file_num}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, object Body)> _appendMessagesToConversationFieldRslugPrefixKbKbidSlugRslugConversationFieldIdMessagesPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/conversation/{param.Params}/messages"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, FileField Body)> _addResourceFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, FileField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/file/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string rslug, string fieldId, bool inline)> _downloadFieldFileRslugPrefixKbKbidSlugRslugFileFieldIdDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rslug, string fieldId, bool inline)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/file/{param.field_id}/download/field?inline={param.inline}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, string rslug, string field) Params, object Body)> _tusPostRslugPrefixKbKbidSlugRslugFileFieldTusuploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string rslug, string field) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/file/{param.Params}/tusupload"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<object, string, ((string kbid, string rslug, string field, string uploadId) Params, object Body)> _tusPatchRslugPrefixKbKbidSlugRslugFileFieldTusuploadUploadIdPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string rslug, string field, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/file/{param.Params}/tusupload/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static HEADAsync<object, string, (string kbid, string rslug, string field, string uploadId)> _uploadInformationKbKbidSlugRslugFileFieldTusuploadUploadIdHead { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHEAD<object, string, (string kbid, string rslug, string field, string uploadId)>(
            url: BaseUrl,
            buildRequest: static (kbid, rslug, field, uploadId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/slug/{rslug}/file/{field}/tusupload/{upload_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceFileUploaded, string, ((string kbid, string rslug, string field) Params, object Body)> _uploadRslugPrefixKbKbidSlugRslugFileFieldUploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, string, ((string kbid, string rslug, string field) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/file/{param.Params}/upload"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, LinkField Body)> _addResourceFieldLinkRslugPrefixKbKbidSlugRslugLinkFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, LinkField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/link/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, ((string kbid, string rslug) Params, object Body)> _reindexResourceRslugPrefixKbKbidSlugRslugReindexPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, ((string kbid, string rslug) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/reindex"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceUpdated, string, ((string kbid, string rslug) Params, object Body)> _reprocessResourceRslugPrefixKbKbidSlugRslugReprocessPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceUpdated, string, ((string kbid, string rslug) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/reprocess"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceUpdated>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, TextField Body)> _addResourceFieldTextRslugPrefixKbKbidSlugRslugTextFieldIdPut { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<ResourceFieldAdded, string, ((string kbid, string rslug, string fieldId) Params, TextField Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/text/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFieldAdded>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId)> _deleteResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId)>(
            url: BaseUrl,
            buildRequest: static (kbid, rslug, fieldType, fieldId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/slug/{rslug}/{field_type}/{field_id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<ResourceField, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)> _getResourceFieldRslugPrefixKbKbidSlugRslugFieldTypeFieldIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResourceField, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, List<ResourceFieldProperties> show, List<ExtractedDataTypeName> extracted, object page)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/slug/{param.rslug}/{param.field_type}/{param.field_id}?show={param.show}&extracted={param.extracted}&page={param.page}"), null, null),
            deserializeSuccess: DeserializeJson<ResourceField>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, string downloadField)> _downloadExtractFileRslugPrefixKbKbidSlugRslugFieldTypeFieldIdDownloadExtractedDownloadFieldGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string rslug, FieldTypeName fieldType, string fieldId, string downloadField)>(
            url: BaseUrl,
            buildRequest: static (kbid, rslug, fieldType, fieldId, downloadField) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/slug/{rslug}/{field_type}/{field_id}/download/extracted/{download_field}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<SyncAskResponse, string, ((string kbid, string slug) Params, AskRequest Body)> _resourceAskEndpointBySlugKbKbidSlugSlugAskPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SyncAskResponse, string, ((string kbid, string slug) Params, AskRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/ask"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<SyncAskResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceAgentsResponse, string, ((string kbid, string slug) Params, ResourceAgentsRequest Body)> _runAgentsBySlugKbKbidSlugSlugRunAgentsPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceAgentsResponse, string, ((string kbid, string slug) Params, ResourceAgentsRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/slug/{param.Params}/run-agents"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceAgentsResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<string, string, (string Params, SplitConfiguration Body)> _addSplitStrategyKbKbidSplitStrategiesPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<string, string, (string Params, SplitConfiguration Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/split_strategies"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<string>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, string> _getSplitStrategiesKbKbidSplitStrategiesGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, string>(
            url: BaseUrl,
            buildRequest: static kbid => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/split_strategies"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, (string kbid, string strategyId)> _deleteSplitStrategyKbKbidSplitStrategiesStrategyStrategyIdDelete { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static (kbid, strategyId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/split_strategies/strategy/{strategy_id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<object, string, (string kbid, string strategyId)> _getSplitStrategyFromIdKbKbidSplitStrategiesStrategyStrategyIdGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, (string kbid, string strategyId)>(
            url: BaseUrl,
            buildRequest: static (kbid, strategyId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/split_strategies/strategy/{strategy_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<KnowledgeboxSuggestResults, string, (string kbid, string query, List<string> fields, List<string> filters, List<string> faceted, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SuggestOptions> features, List<ResourceProperties> show, List<FieldTypeName> fieldType, bool debug, bool highlight, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)> _suggestKnowledgeboxKbKbidSuggestGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<KnowledgeboxSuggestResults, string, (string kbid, string query, List<string> fields, List<string> filters, List<string> faceted, object rangeCreationStart, object rangeCreationEnd, object rangeModificationStart, object rangeModificationEnd, List<SuggestOptions> features, List<ResourceProperties> show, List<FieldTypeName> fieldType, bool debug, bool highlight, bool showHidden, NucliaDBClientType xNdbClient, string xNucliadbUser, string xForwardedFor)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/suggest?query={param.query}&fields={param.fields}&filters={param.filters}&faceted={param.faceted}&range_creation_start={param.rangeCreationStart}&range_creation_end={param.rangeCreationEnd}&range_modification_start={param.rangeModificationStart}&range_modification_end={param.rangeModificationEnd}&features={param.features}&show={param.show}&field_type={param.fieldType}&debug={param.debug}&highlight={param.highlight}&show_hidden={param.showHidden}&x-ndb-client={param.xNdbClient}&x-nucliadb-user={param.xNucliadbUser}&x-forwarded-for={param.xForwardedFor}"), null, null),
            deserializeSuccess: DeserializeJson<KnowledgeboxSuggestResults>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<SummarizedResponse, string, (string Params, SummarizeRequest Body)> _summarizeEndpointKbKbidSummarizePost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<SummarizedResponse, string, (string Params, SummarizeRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/summarize"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<SummarizedResponse>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<object, string, (string Params, object Body)> _tusPostKbKbidTusuploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/tusupload"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static OPTIONSAsync<object, string, (string kbid, object rid, object rslug, object uploadId, object field)> _tusOptionsKbKbidTusuploadOptions { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateOPTIONS<object, string, (string kbid, object rid, object rslug, object uploadId, object field)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.kbid}/tusupload?rid={param.rid}&rslug={param.rslug}&upload_id={param.uploadId}&field={param.field}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PatchAsync<object, string, ((string kbid, string uploadId) Params, object Body)> _patchKbKbidTusuploadUploadIdPatch { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, string, ((string kbid, string uploadId) Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/tusupload/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static HEADAsync<object, string, (string kbid, string uploadId)> _uploadInformationKbKbidTusuploadUploadIdHead { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateHEAD<object, string, (string kbid, string uploadId)>(
            url: BaseUrl,
            buildRequest: static (kbid, uploadId) => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{kbid}/tusupload/{upload_id}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<ResourceFileUploaded, string, (string Params, object Body)> _uploadKbKbidUploadPost { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResourceFileUploaded, string, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/api/v1/kb/{param.Params}/upload"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<ResourceFileUploaded>,
            deserializeError: DeserializeError
        );

    #endregion

    #region Learning Operations

    private static GetAsync<object, string, Unit> _learningConfigurationSchemaLearningConfigurationSchemaGet { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/api/v1/learning/configuration/schema"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    #endregion

    private static ProgressReportingHttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        System.Console.WriteLine($"[DEBUG] Serializing request: {json}");
        return new ProgressReportingHttpContent(json, contentType: "application/json");
    }

    private static async Task<T> DeserializeJson<T>(
        HttpResponseMessage response,
        CancellationToken ct = default
    )
    {
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        System.Console.WriteLine($"[DEBUG] Response status: {response.StatusCode}, URL: {response.RequestMessage?.RequestUri}, body: {body}");
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken: ct).ConfigureAwait(false);
        return result ?? throw new InvalidOperationException($"Failed to deserialize response to type {typeof(T).Name}");
    }

    private static async Task<string> DeserializeString(
        HttpResponseMessage response,
        CancellationToken ct = default
    ) =>
        await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

    private static async Task<string> DeserializeError(
        HttpResponseMessage response,
        CancellationToken ct = default
    )
    {
        var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        return string.IsNullOrEmpty(content) ? "Unknown error" : content;
    }
}