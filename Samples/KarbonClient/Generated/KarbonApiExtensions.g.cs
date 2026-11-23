#nullable enable
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

namespace Karbon.Generated;

/// <summary>Extension methods for API operations.</summary>
public static class KarbonApiExtensions
{
    #region Configuration

    private static readonly AbsoluteUrl BaseUrl = "https://api.karbonhq.com".ToAbsoluteUrl();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
        Task.FromResult(Unit.Value);

    #endregion

    #region V3 Operations

    /// <summary>Use the `GET` method on this endpoint to receive a list of the integration task types that have been defined for your integration</summary>
    public static Task<Result<IntegrationTaskDefinitions, HttpError<string>>> ListIntegrationTaskDefintionsAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _listIntegrationTaskDefintionsAsync(httpClient, Unit.Value, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a list of the integration tasks created in the Karbon account which has connected your integration</summary>
    public static Task<Result<IntegrationTasks, HttpError<string>>> ListIntegrationTasksAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "",
        CancellationToken cancellationToken = default
    ) => _listIntegrationTasksAsync(httpClient, (filter, orderby), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to retrieve the details of a single integration task</summary>
    public static Task<Result<IntegrationTask, HttpError<ErrorMessages>>> GetIntegrationTaskAsync(
        this HttpClient httpClient,
        string integrationTaskKey,
        CancellationToken cancellationToken = default
    ) => _getIntegrationTaskAsync(httpClient, integrationTaskKey, cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update the details of a single integration task</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> UpdateIntegrationTaskAsync(
        this HttpClient httpClient,
        string integrationTaskKey, IntegrationTaskWriteable body,
        CancellationToken cancellationToken = default
    ) => _updateIntegrationTaskAsync(httpClient, (integrationTaskKey, body), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a list of Invoices</summary>
    public static Task<Result<object, HttpError<ResourceNotFound>>> GetInvoicesAsync(
        this HttpClient httpClient,
        int? top = null, int? skip = null, string orderby = "",
        CancellationToken cancellationToken = default
    ) => _getInvoicesAsync(httpClient, (top, skip, orderby), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of an Invoice specified using the InvoiceKey.</summary>
    public static Task<Result<object, HttpError<ResourceNotFound>>> GetInvoiceByKeyAsync(
        this HttpClient httpClient,
        string invoiceKey, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getInvoiceByKeyAsync(httpClient, (invoiceKey, expand), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a list of Payments</summary>
    public static Task<Result<object, HttpError<ResourceNotFound>>> GetPaymentsAsync(
        this HttpClient httpClient,
        int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getPaymentsAsync(httpClient, (top, skip), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a Manual Payment</summary>
    public static Task<Result<object, HttpError<string>>> CreateManualPaymentAsync(
        this HttpClient httpClient,
        object body,
        CancellationToken cancellationToken = default
    ) => _createManualPaymentAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `DELETE` method on this endpoint to delete a Manual Payment</summary>
    public static Task<Result<Unit, HttpError<ResourceNotFound>>> DeleteManualPaymentAsync(
        this HttpClient httpClient,
        string manualPaymentKey,
        CancellationToken cancellationToken = default
    ) => _deleteManualPaymentAsync(httpClient, manualPaymentKey, cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to reverse a Manual Payment</summary>
    public static Task<Result<object, HttpError<ResourceNotFound>>> ReverseManualPaymentAsync(
        this HttpClient httpClient,
        object body,
        CancellationToken cancellationToken = default
    ) => _reverseManualPaymentAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive details of a single Payment</summary>
    public static Task<Result<object, HttpError<ResourceNotFound>>> GetPaymentByPaymentKeyAsync(
        this HttpClient httpClient,
        string paymentKey,
        CancellationToken cancellationToken = default
    ) => _getPaymentByPaymentKeyAsync(httpClient, paymentKey, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Business Card specified using the BusinessCardKey.</summary>
    public static Task<Result<object, HttpError<ResourceNotFound>>> GetBusinessCardByKeyAsync(
        this HttpClient httpClient,
        string businessCardKey,
        CancellationToken cancellationToken = default
    ) => _getBusinessCardByKeyAsync(httpClient, businessCardKey, cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update the details of a Business Card specified using the BusinessCardKey.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> UpdateBusinessCardByKeyAsync(
        this HttpClient httpClient,
        string businessCardKey, object body,
        CancellationToken cancellationToken = default
    ) => _updateBusinessCardByKeyAsync(httpClient, (businessCardKey, body), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Client Groups from your tenant. Using the query parameters available to this endpoint, you can also filter the list of Client Groups by their full name.</summary>
    public static Task<Result<GetClientGroup, HttpError<ErrorMessages>>> GetClientGroupsAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "ClientGroupKey", int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getClientGroupsAsync(httpClient, (filter, orderby, top, skip), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new Client Group in your tenant.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> CreateClientGroupAsync(
        this HttpClient httpClient,
        object body,
        CancellationToken cancellationToken = default
    ) => _createClientGroupAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Client Group specified using the UserDefinedIdentifier.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetClientGroupByUDIAsync(
        this HttpClient httpClient,
        string userDefinedIdentifier, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getClientGroupByUDIAsync(httpClient, (userDefinedIdentifier, expand), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Client Group specified using the `ClientGroupkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetClientGroupByIDAsync(
        this HttpClient httpClient,
        string clientGroupkey, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getClientGroupByIDAsync(httpClient, (clientGroupkey, expand), cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update full details of a Client Group specified using the `ClientGroupkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PutClientGroupByIDAsync(
        this HttpClient httpClient,
        string clientGroupkey, object body,
        CancellationToken cancellationToken = default
    ) => _putClientGroupByIDAsync(httpClient, (clientGroupkey, body), cancellationToken);
    
    /// <summary>Use the `PATCH` method on this endpoint to update partial details of a Client Group specified using the `ClientGroupkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PatchClientGroupByIDAsync(
        this HttpClient httpClient,
        string clientGroupkey, object body,
        CancellationToken cancellationToken = default
    ) => _patchClientGroupByIDAsync(httpClient, (clientGroupkey, body), cancellationToken);
    
    /// <summary>This endpoint can be used to get the comment attached to any Karbon entity (e.g., Work Item, Timesheet, Notes, Contacts).</summary>
    public static Task<Result<GetComment, HttpError<ErrorMessages>>> GetCommentAsync(
        this HttpClient httpClient,
        string commentkey,
        CancellationToken cancellationToken = default
    ) => _getCommentAsync(httpClient, commentkey, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Contacts from your tenant.   Using the query parameters available to this endpoint,  you can also filter the list of Contacts by their full name, email address, or phone number.</summary>
    public static Task<Result<GetContacts, HttpError<ErrorMessages>>> GetAllContactsAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "ClientGroupKey", int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getAllContactsAsync(httpClient, (filter, orderby, top, skip), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new Contact in your tenant.</summary>
    public static Task<Result<ResponseCreateContact, HttpError<ErrorMessages>>> CreateContactAsync(
        this HttpClient httpClient,
        CreateContact body,
        CancellationToken cancellationToken = default
    ) => _createContactAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Contact specified using the UserDefinedIdentifier.</summary>
    public static Task<Result<ResponseCreateContact, HttpError<ErrorMessages>>> GetContactByUDIAsync(
        this HttpClient httpClient,
        string userDefinedIdentifier, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getContactByUDIAsync(httpClient, (userDefinedIdentifier, expand), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Contact specified using the `Contactkey`.</summary>
    public static Task<Result<ResponseCreateContact, HttpError<ErrorMessages>>> GetContactByIDAsync(
        this HttpClient httpClient,
        string contactkey, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getContactByIDAsync(httpClient, (contactkey, expand), cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update full details of a Contact specified using the `Contactkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PutContactByIDAsync(
        this HttpClient httpClient,
        string contactkey, CreateContact body,
        CancellationToken cancellationToken = default
    ) => _putContactByIDAsync(httpClient, (contactkey, body), cancellationToken);
    
    /// <summary>Use the `PATCH` method on this endpoint to update partial details of a Contact specified using the `Contactkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PatchContactsByIDAsync(
        this HttpClient httpClient,
        string contactkey, object body,
        CancellationToken cancellationToken = default
    ) => _patchContactsByIDAsync(httpClient, (contactkey, body), cancellationToken);
    
    /// <summary>Retrieve a list of all of the custom fields that have been created for a Karbon account</summary>
    public static Task<Result<CustomFieldDefinitions, HttpError<string>>> GetCustomFieldDefinitionsAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _getCustomFieldDefinitionsAsync(httpClient, Unit.Value, cancellationToken);
    
    /// <summary>Defines a new custom field that can be assigned to a Contact and/or Organization - use the Karbon UI to create examples of how a new field with specific list options should be created</summary>
    public static Task<Result<CustomFieldDefinition, HttpError<ErrorMessages>>> CreateCustomFieldDefinitionAsync(
        this HttpClient httpClient,
        CustomFieldDefinition body,
        CancellationToken cancellationToken = default
    ) => _createCustomFieldDefinitionAsync(httpClient, body, cancellationToken);
    
    /// <summary>Delete a custom field defintion by key</summary>
    public static Task<Result<Unit, HttpError<ErrorMessages>>> DeleteCustomFieldDefinitionAsync(
        this HttpClient httpClient,
        string customFieldDefinitionKey,
        CancellationToken cancellationToken = default
    ) => _deleteCustomFieldDefinitionAsync(httpClient, customFieldDefinitionKey, cancellationToken);
    
    /// <summary>Retrives the custom fields for a Contact or Organization</summary>
    public static Task<Result<CustomFieldValues, HttpError<ErrorMessages>>> GetCustomFieldValuesAsync(
        this HttpClient httpClient,
        string entityKey,
        CancellationToken cancellationToken = default
    ) => _getCustomFieldValuesAsync(httpClient, entityKey, cancellationToken);
    
    /// <summary>Performs an update of the complete set of custom field values for a Contact or Organization</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> UpdateCustomFieldValuesAsync(
        this HttpClient httpClient,
        string entityKey, CustomFieldValues body,
        CancellationToken cancellationToken = default
    ) => _updateCustomFieldValuesAsync(httpClient, (entityKey, body), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the estimate summaries of a Work Item specified using the `WorkItemkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetEstimateSummariesByWorkItemKeyAsync(
        this HttpClient httpClient,
        string workItemkey,
        CancellationToken cancellationToken = default
    ) => _getEstimateSummariesByWorkItemKeyAsync(httpClient, workItemkey, cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to upload and link a file to an entity in your tenant.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> CreateFileAsync(
        this HttpClient httpClient,
        object body,
        CancellationToken cancellationToken = default
    ) => _createFileAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint with the `token` query string parameter to retrieve a file. Note: download tokens are only valid for 15 minutes from the moment of issue.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> DownloadFileAsync(
        this HttpClient httpClient,
        string token,
        CancellationToken cancellationToken = default
    ) => _downloadFileAsync(httpClient, token, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to list files associated with a specific Entity Type and Entity Key</summary>
    public static Task<Result<FileList, HttpError<ErrorMessages>>> ListFilesAsync(
        this HttpClient httpClient,
        string entityType, string entityKey,
        CancellationToken cancellationToken = default
    ) => _listFilesAsync(httpClient, (entityType, entityKey), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the Note specified using the `NoteID`.</summary>
    public static Task<Result<NoteResponse, HttpError<ErrorMessages>>> GetNoteByNoteKeyAsync(
        this HttpClient httpClient,
        string noteID,
        CancellationToken cancellationToken = default
    ) => _getNoteByNoteKeyAsync(httpClient, noteID, cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new Note and associate it with a Karbon entity (e.g., WorkItem, Contact, Organization, or ClientGroup) in your tenant.</summary>
    public static Task<Result<NoteResponse, HttpError<ErrorMessages>>> CreateNoteAsync(
        this HttpClient httpClient,
        NoteRequest body,
        CancellationToken cancellationToken = default
    ) => _createNoteAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Organizations from your tenant.   Using the query parameters available to this endpoint,  you can also filter the list of Organizations by their full name, or email address.</summary>
    public static Task<Result<GetOrganizations, HttpError<ErrorMessages>>> GetAllOrganizationsAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "OrganizationKey", int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getAllOrganizationsAsync(httpClient, (filter, orderby, top, skip), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new Organization in your tenant.</summary>
    public static Task<Result<CreateOrganizationResponse, HttpError<ErrorMessages>>> CreateOrganizationAsync(
        this HttpClient httpClient,
        CreateOrganization body,
        CancellationToken cancellationToken = default
    ) => _createOrganizationAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Organization specified using the UserDefinedIdentifier.</summary>
    public static Task<Result<WithBusinessCardsCreateOrganizationResponse, HttpError<ErrorMessages>>> GetOrganizationByUDIAsync(
        this HttpClient httpClient,
        string userDefinedIdentifier, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getOrganizationByUDIAsync(httpClient, (userDefinedIdentifier, expand), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Organization specified using the `Organizationkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetOrganizationByIDAsync(
        this HttpClient httpClient,
        string organizationkey, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getOrganizationByIDAsync(httpClient, (organizationkey, expand), cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update full details of a Organization specified using the `Organizationkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PutOrganizationByIDAsync(
        this HttpClient httpClient,
        string organizationkey, UpdateOrganization body,
        CancellationToken cancellationToken = default
    ) => _putOrganizationByIDAsync(httpClient, (organizationkey, body), cancellationToken);
    
    /// <summary>Use the `PATCH` method on this endpoint to update partial details of a Organization specified using the `Organizationkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PatchOrganizationByIDAsync(
        this HttpClient httpClient,
        string organizationkey, object body, string? prefer = null,
        CancellationToken cancellationToken = default
    ) => _patchOrganizationByIDAsync(httpClient, (organizationkey, prefer, body), cancellationToken);
    
    /// <summary>Get specific settings for a tenant, including the valid work statuses, work types and client types</summary>
    public static Task<Result<GetTenantSettings, HttpError<ResourceNotFound>>> GetTenantSettingsAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _getTenantSettingsAsync(httpClient, Unit.Value, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Timesheets from your tenant. Using the query parameters available to this endpoint,  you can also filter the list of Timesheets by their StartDate,  EndDate, UserKey, WorkItemKeys, and Status.</summary>
    public static Task<Result<GetTimeSheets, HttpError<ErrorMessages>>> GetAllTimesheetsAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "TimesheetKey", int? top = null, int? skip = null, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getAllTimesheetsAsync(httpClient, (filter, orderby, top, skip, expand), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Timesheet specified using the `Timesheetkey`.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetTimesheetByIDAsync(
        this HttpClient httpClient,
        string timesheetkey, string expand = "",
        CancellationToken cancellationToken = default
    ) => _getTimesheetByIDAsync(httpClient, (timesheetkey, expand), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Users from your tenant.   Using the query parameters available to this endpoint, you can also filter the list of Users by their Name, and EmailAddress.</summary>
    public static Task<Result<GetUsers, HttpError<ErrorMessages>>> GetAllUsersAsync(
        this HttpClient httpClient,
        string filter = "", int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getAllUsersAsync(httpClient, (filter, top, skip), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new User in your tenant.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> CreateUserAsync(
        this HttpClient httpClient,
        object body,
        CancellationToken cancellationToken = default
    ) => _createUserAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a single User from your tenant.</summary>
    public static Task<Result<GetUserById, HttpError<ErrorMessages>>> GetUserByIdAsync(
        this HttpClient httpClient,
        string userId,
        CancellationToken cancellationToken = default
    ) => _getUserByIdAsync(httpClient, userId, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Webhook Subscriptions associated with the Karbon entity specified using the `WebhookType`.</summary>
    public static Task<Result<GetWebhookSubscriptions, HttpError<ResourceNotFound>>> GetWebhookSubscriptionsByWebhookTypeAsync(
        this HttpClient httpClient,
        string webhookType,
        CancellationToken cancellationToken = default
    ) => _getWebhookSubscriptionsByWebhookTypeAsync(httpClient, webhookType, cancellationToken);
    
    /// <summary>Use the `DELETE` method on this endpoint to delete a Webhook Subscription associated with the Karbon entity specified using the `WebhookType`.</summary>
    public static Task<Result<Unit, HttpError<ResourceNotFound>>> DelWebhookSubscriptionsByWebhookTypeAsync(
        this HttpClient httpClient,
        string webhookType,
        CancellationToken cancellationToken = default
    ) => _delWebhookSubscriptionsByWebhookTypeAsync(httpClient, webhookType, cancellationToken);
    
    /// <summary>Use the `POST` method on create and associate a Webhook Subscriptions with a Karbon entity.</summary>
    public static Task<Result<GetWebhookSubscriptions, HttpError<ErrorMessages>>> CreateWebhookSubscriptionAsync(
        this HttpClient httpClient,
        object body,
        CancellationToken cancellationToken = default
    ) => _createWebhookSubscriptionAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `DELETE` method on this endpoint to delete all Webhook Subscriptions associated with Contact, Work, and Note.</summary>
    public static Task<Result<Unit, HttpError<ResourceNotFound>>> DelAllWebhookSubscriptionsAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _delAllWebhookSubscriptionsAsync(httpClient, Unit.Value, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Work Items from your tenant. Using the query parameters available to this endpoint, you can also filter the list of Work Items by their AssigneeEmailAddress, ClientKey, PrimaryStatus StartDate, Title, WorkStatus, and WorkType.</summary>
    public static Task<Result<GetWorkItems, HttpError<ErrorMessages>>> GetAllWorkItemsAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "WorkItemKey", int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getAllWorkItemsAsync(httpClient, (filter, orderby, top, skip), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new Work Item in your tenant.</summary>
    public static Task<Result<object, HttpError<string>>> CreateWorkItemAsync(
        this HttpClient httpClient,
        RequestCreateWorkItem body,
        CancellationToken cancellationToken = default
    ) => _createWorkItemAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Work Item specified using the WorkItemkey.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetWorkItemByIDAsync(
        this HttpClient httpClient,
        string workItemkey,
        CancellationToken cancellationToken = default
    ) => _getWorkItemByIDAsync(httpClient, workItemkey, cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update full details of a Work Item (specified using the WorkItemkey).</summary>
    public static Task<Result<object, HttpError<string>>> PutWorkItemByIDAsync(
        this HttpClient httpClient,
        string workItemkey, RequestCreateWorkItem body,
        CancellationToken cancellationToken = default
    ) => _putWorkItemByIDAsync(httpClient, (workItemkey, body), cancellationToken);
    
    /// <summary>Use the `PATCH` method on this endpoint to update partial details of a Work Item (specified using the WorkItemkey).</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PatchWorkItemByIDAsync(
        this HttpClient httpClient,
        string workItemkey, UpdateWorkItem body,
        CancellationToken cancellationToken = default
    ) => _patchWorkItemByIDAsync(httpClient, (workItemkey, body), cancellationToken);
    
    /// <summary>Use the `POST` method on this endpoint to create a new Work Schedule in your tenant.</summary>
    public static Task<Result<GetWorkSchedule, HttpError<ErrorMessages>>> CreateWorkScheduleAsync(
        this HttpClient httpClient,
        WorkScheduleRequest body,
        CancellationToken cancellationToken = default
    ) => _createWorkScheduleAsync(httpClient, body, cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Work Schedule specified using the WorkSchedulekey.</summary>
    public static Task<Result<GetWorkSchedule, HttpError<ErrorMessages>>> GetWorkScheduleByIDAsync(
        this HttpClient httpClient,
        string workSchedulekey,
        CancellationToken cancellationToken = default
    ) => _getWorkScheduleByIDAsync(httpClient, workSchedulekey, cancellationToken);
    
    /// <summary>Use the `PUT` method on this endpoint to update full details of a Work Schedule specified using the WorkSchedulekey.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> PutWorkScheduleByIDAsync(
        this HttpClient httpClient,
        string workSchedulekey, WorkSchedulePUTRequest body,
        CancellationToken cancellationToken = default
    ) => _putWorkScheduleByIDAsync(httpClient, (workSchedulekey, body), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Work Templates from your tenant. Using the query parameters available to this endpoint, you can also filter the list of Work Templates by their Title, WorkTypeKey, HasScheduledClientTaskGroups, DraftHasChanges, PublishedDate, NumberOfWorkItemsCreated, DateLastWorkItemCreated, or DateModified.</summary>
    public static Task<Result<GetWorkTemplates, HttpError<ErrorMessages>>> GetAllWorkTemplatesAsync(
        this HttpClient httpClient,
        string filter = "", string orderby = "WorkTypeKey", int? top = null, int? skip = null,
        CancellationToken cancellationToken = default
    ) => _getAllWorkTemplatesAsync(httpClient, (filter, orderby, top, skip), cancellationToken);
    
    /// <summary>Use the `GET` method on this endpoint to receive the details of a Work Template specified using the WorkTemplatekey.</summary>
    public static Task<Result<object, HttpError<ErrorMessages>>> GetWorkTemplateByIDAsync(
        this HttpClient httpClient,
        string workTemplatekey,
        CancellationToken cancellationToken = default
    ) => _getWorkTemplateByIDAsync(httpClient, workTemplatekey, cancellationToken);

    #endregion

    private static GetAsync<IntegrationTaskDefinitions, string, Unit> _listIntegrationTaskDefintionsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<IntegrationTaskDefinitions, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/v3/IntegrationTaskDefinitions"), null, null),
            deserializeSuccess: DeserializeJson<IntegrationTaskDefinitions>,
            deserializeError: DeserializeError
        );

    private static GetAsync<IntegrationTasks, string, (string filter, string orderby)> _listIntegrationTasksAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<IntegrationTasks, string, (string filter, string orderby)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/IntegrationTasks?$filter={param.filter}&$orderby={param.orderby}"), null, null),
            deserializeSuccess: DeserializeJson<IntegrationTasks>,
            deserializeError: DeserializeError
        );

    private static GetAsync<IntegrationTask, ErrorMessages, string> _getIntegrationTaskAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<IntegrationTask, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static integrationTaskKey => new HttpRequestParts(new RelativeUrl($"/v3/IntegrationTasks/{integrationTaskKey}"), null, null),
            deserializeSuccess: DeserializeJson<IntegrationTask>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, ErrorMessages, (string Params, IntegrationTaskWriteable Body)> _updateIntegrationTaskAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, IntegrationTaskWriteable Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/IntegrationTasks/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ResourceNotFound, (int? top, int? skip, string orderby)> _getInvoicesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ResourceNotFound, (int? top, int? skip, string orderby)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Invoices?$top={param.top}&$skip={param.skip}&$orderby={param.orderby}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static GetAsync<object, ResourceNotFound, (string invoiceKey, string expand)> _getInvoiceByKeyAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ResourceNotFound, (string invoiceKey, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Invoices/{param.invoiceKey}?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static GetAsync<object, ResourceNotFound, (int? top, int? skip)> _getPaymentsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ResourceNotFound, (int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Payments?$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static PostAsync<object, string, object> _createManualPaymentAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, object>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/ManualPayments"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, ResourceNotFound, string> _deleteManualPaymentAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, ResourceNotFound, string>(
            url: BaseUrl,
            buildRequest: static manualPaymentKey => new HttpRequestParts(new RelativeUrl($"/v3/ManualPayments/{manualPaymentKey}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static PostAsync<object, ResourceNotFound, object> _reverseManualPaymentAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, ResourceNotFound, object>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/ReverseManualPayment"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static GetAsync<object, ResourceNotFound, string> _getPaymentByPaymentKeyAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ResourceNotFound, string>(
            url: BaseUrl,
            buildRequest: static paymentKey => new HttpRequestParts(new RelativeUrl($"/v3/Payments/{paymentKey}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static GetAsync<object, ResourceNotFound, string> _getBusinessCardByKeyAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ResourceNotFound, string>(
            url: BaseUrl,
            buildRequest: static businessCardKey => new HttpRequestParts(new RelativeUrl($"/v3/BusinessCards/{businessCardKey}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static PutAsync<object, ErrorMessages, (string Params, object Body)> _updateBusinessCardByKeyAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/BusinessCards/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetClientGroup, ErrorMessages, (string filter, string orderby, int? top, int? skip)> _getClientGroupsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetClientGroup, ErrorMessages, (string filter, string orderby, int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/ClientGroups?$filter={param.filter}&$orderby={param.orderby}&$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<GetClientGroup>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<object, ErrorMessages, object> _createClientGroupAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, ErrorMessages, object>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/ClientGroups"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, (string userDefinedIdentifier, string expand)> _getClientGroupByUDIAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, (string userDefinedIdentifier, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/ClientGroups/GetClientGroupByUserDefinedIdentifier(UserDefinedIdentifier='{param.userDefinedIdentifier}')?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, (string clientGroupkey, string expand)> _getClientGroupByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, (string clientGroupkey, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/ClientGroups/{param.clientGroupkey}?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, ErrorMessages, (string Params, object Body)> _putClientGroupByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/ClientGroups/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PatchAsync<object, ErrorMessages, (string Params, object Body)> _patchClientGroupByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, ErrorMessages, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/ClientGroups/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetComment, ErrorMessages, string> _getCommentAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetComment, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static commentkey => new HttpRequestParts(new RelativeUrl($"/v3/Comments('{commentkey}')"), null, null),
            deserializeSuccess: DeserializeJson<GetComment>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetContacts, ErrorMessages, (string filter, string orderby, int? top, int? skip)> _getAllContactsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetContacts, ErrorMessages, (string filter, string orderby, int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Contacts?$filter={param.filter}&$orderby={param.orderby}&$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<GetContacts>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<ResponseCreateContact, ErrorMessages, CreateContact> _createContactAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<ResponseCreateContact, ErrorMessages, CreateContact>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/Contacts"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<ResponseCreateContact>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<ResponseCreateContact, ErrorMessages, (string userDefinedIdentifier, string expand)> _getContactByUDIAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResponseCreateContact, ErrorMessages, (string userDefinedIdentifier, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Contacts/GetContactByUserDefinedIdentifier(UserDefinedIdentifier='{param.userDefinedIdentifier}')?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<ResponseCreateContact>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<ResponseCreateContact, ErrorMessages, (string contactkey, string expand)> _getContactByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<ResponseCreateContact, ErrorMessages, (string contactkey, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Contacts/{param.contactkey}?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<ResponseCreateContact>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, ErrorMessages, (string Params, CreateContact Body)> _putContactByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, CreateContact Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Contacts/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PatchAsync<object, ErrorMessages, (string Params, object Body)> _patchContactsByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, ErrorMessages, (string Params, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Contacts/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<CustomFieldDefinitions, string, Unit> _getCustomFieldDefinitionsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<CustomFieldDefinitions, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/v3/CustomFields"), null, null),
            deserializeSuccess: DeserializeJson<CustomFieldDefinitions>,
            deserializeError: DeserializeError
        );

    private static PostAsync<CustomFieldDefinition, ErrorMessages, CustomFieldDefinition> _createCustomFieldDefinitionAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CustomFieldDefinition, ErrorMessages, CustomFieldDefinition>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/CustomFields"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<CustomFieldDefinition>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static DeleteAsync<Unit, ErrorMessages, string> _deleteCustomFieldDefinitionAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static customFieldDefinitionKey => new HttpRequestParts(new RelativeUrl($"/v3/CustomFields/{customFieldDefinitionKey}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<CustomFieldValues, ErrorMessages, string> _getCustomFieldValuesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<CustomFieldValues, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static entityKey => new HttpRequestParts(new RelativeUrl($"/v3/CustomFieldValues/{entityKey}"), null, null),
            deserializeSuccess: DeserializeJson<CustomFieldValues>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, ErrorMessages, (string Params, CustomFieldValues Body)> _updateCustomFieldValuesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, CustomFieldValues Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/CustomFieldValues/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, string> _getEstimateSummariesByWorkItemKeyAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static workItemkey => new HttpRequestParts(new RelativeUrl($"/v3/EstimateSummaries/{workItemkey}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<object, ErrorMessages, object> _createFileAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, ErrorMessages, object>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/Files"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, string> _downloadFileAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Files?token={param}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<FileList, ErrorMessages, (string entityType, string entityKey)> _listFilesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<FileList, ErrorMessages, (string entityType, string entityKey)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/FileList/{param.entityType}?EntityKey={param.entityKey}"), null, null),
            deserializeSuccess: DeserializeJson<FileList>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<NoteResponse, ErrorMessages, string> _getNoteByNoteKeyAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<NoteResponse, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static noteID => new HttpRequestParts(new RelativeUrl($"/v3/Notes/{noteID}"), null, null),
            deserializeSuccess: DeserializeJson<NoteResponse>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<NoteResponse, ErrorMessages, NoteRequest> _createNoteAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<NoteResponse, ErrorMessages, NoteRequest>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/Notes"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<NoteResponse>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetOrganizations, ErrorMessages, (string filter, string orderby, int? top, int? skip)> _getAllOrganizationsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetOrganizations, ErrorMessages, (string filter, string orderby, int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Organizations?$filter={param.filter}&$orderby={param.orderby}&$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<GetOrganizations>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<CreateOrganizationResponse, ErrorMessages, CreateOrganization> _createOrganizationAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<CreateOrganizationResponse, ErrorMessages, CreateOrganization>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/Organizations"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<CreateOrganizationResponse>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<WithBusinessCardsCreateOrganizationResponse, ErrorMessages, (string userDefinedIdentifier, string expand)> _getOrganizationByUDIAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<WithBusinessCardsCreateOrganizationResponse, ErrorMessages, (string userDefinedIdentifier, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Organizations/GetOrganizationByUserDefinedIdentifier(UserDefinedIdentifier='{param.userDefinedIdentifier}')?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<WithBusinessCardsCreateOrganizationResponse>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, (string organizationkey, string expand)> _getOrganizationByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, (string organizationkey, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Organizations/{param.organizationkey}?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, ErrorMessages, (string Params, UpdateOrganization Body)> _putOrganizationByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, UpdateOrganization Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Organizations/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PatchAsync<object, ErrorMessages, (string organizationkey, string? prefer, object Body)> _patchOrganizationByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, ErrorMessages, (string organizationkey, string? prefer, object Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Organizations/{param.organizationkey}"), CreateJsonContent(param.Body), new Dictionary<string, string> { ["Prefer"] = param.prefer?.ToString() ?? string.Empty }),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetTenantSettings, ResourceNotFound, Unit> _getTenantSettingsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetTenantSettings, ResourceNotFound, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/v3/TenantSettings"), null, null),
            deserializeSuccess: DeserializeJson<GetTenantSettings>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static GetAsync<GetTimeSheets, ErrorMessages, (string filter, string orderby, int? top, int? skip, string expand)> _getAllTimesheetsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetTimeSheets, ErrorMessages, (string filter, string orderby, int? top, int? skip, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Timesheets?$filter={param.filter}&$orderby={param.orderby}&$top={param.top}&$skip={param.skip}&$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<GetTimeSheets>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, (string timesheetkey, string expand)> _getTimesheetByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, (string timesheetkey, string expand)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Timesheets/{param.timesheetkey}?$expand={param.expand}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetUsers, ErrorMessages, (string filter, int? top, int? skip)> _getAllUsersAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetUsers, ErrorMessages, (string filter, int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/Users?$filter={param.filter}&$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<GetUsers>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<object, ErrorMessages, object> _createUserAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, ErrorMessages, object>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/Users"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetUserById, ErrorMessages, string> _getUserByIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetUserById, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static userId => new HttpRequestParts(new RelativeUrl($"/v3/Users/{userId}"), null, null),
            deserializeSuccess: DeserializeJson<GetUserById>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetWebhookSubscriptions, ResourceNotFound, string> _getWebhookSubscriptionsByWebhookTypeAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetWebhookSubscriptions, ResourceNotFound, string>(
            url: BaseUrl,
            buildRequest: static webhookType => new HttpRequestParts(new RelativeUrl($"/v3/WebhookSubscriptions/{webhookType}"), null, null),
            deserializeSuccess: DeserializeJson<GetWebhookSubscriptions>,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static DeleteAsync<Unit, ResourceNotFound, string> _delWebhookSubscriptionsByWebhookTypeAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, ResourceNotFound, string>(
            url: BaseUrl,
            buildRequest: static webhookType => new HttpRequestParts(new RelativeUrl($"/v3/WebhookSubscriptions/{webhookType}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static PostAsync<GetWebhookSubscriptions, ErrorMessages, object> _createWebhookSubscriptionAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GetWebhookSubscriptions, ErrorMessages, object>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/WebhookSubscriptions"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<GetWebhookSubscriptions>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static DeleteAsync<Unit, ResourceNotFound, Unit> _delAllWebhookSubscriptionsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, ResourceNotFound, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/v3/WebhookSubscriptions"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeJson<ResourceNotFound>
        );

    private static GetAsync<GetWorkItems, ErrorMessages, (string filter, string orderby, int? top, int? skip)> _getAllWorkItemsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetWorkItems, ErrorMessages, (string filter, string orderby, int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/WorkItems?$filter={param.filter}&$orderby={param.orderby}&$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<GetWorkItems>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<object, string, RequestCreateWorkItem> _createWorkItemAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<object, string, RequestCreateWorkItem>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/WorkItems"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static GetAsync<object, ErrorMessages, string> _getWorkItemByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static workItemkey => new HttpRequestParts(new RelativeUrl($"/v3/WorkItems/{workItemkey}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, string, (string Params, RequestCreateWorkItem Body)> _putWorkItemByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, string, (string Params, RequestCreateWorkItem Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/WorkItems/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeError
        );

    private static PatchAsync<object, ErrorMessages, (string Params, UpdateWorkItem Body)> _patchWorkItemByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePatch<object, ErrorMessages, (string Params, UpdateWorkItem Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/WorkItems/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PostAsync<GetWorkSchedule, ErrorMessages, WorkScheduleRequest> _createWorkScheduleAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<GetWorkSchedule, ErrorMessages, WorkScheduleRequest>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/v3/WorkSchedules"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<GetWorkSchedule>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetWorkSchedule, ErrorMessages, string> _getWorkScheduleByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetWorkSchedule, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static workSchedulekey => new HttpRequestParts(new RelativeUrl($"/v3/WorkSchedules/{workSchedulekey}"), null, null),
            deserializeSuccess: DeserializeJson<GetWorkSchedule>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static PutAsync<object, ErrorMessages, (string Params, WorkSchedulePUTRequest Body)> _putWorkScheduleByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<object, ErrorMessages, (string Params, WorkSchedulePUTRequest Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/WorkSchedules/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<GetWorkTemplates, ErrorMessages, (string filter, string orderby, int? top, int? skip)> _getAllWorkTemplatesAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<GetWorkTemplates, ErrorMessages, (string filter, string orderby, int? top, int? skip)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/v3/WorkTemplates?$filter={param.filter}&$orderby={param.orderby}&$top={param.top}&$skip={param.skip}"), null, null),
            deserializeSuccess: DeserializeJson<GetWorkTemplates>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static GetAsync<object, ErrorMessages, string> _getWorkTemplateByIDAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<object, ErrorMessages, string>(
            url: BaseUrl,
            buildRequest: static workTemplatekey => new HttpRequestParts(new RelativeUrl($"/v3/WorkTemplates/{workTemplatekey}"), null, null),
            deserializeSuccess: DeserializeJson<object>,
            deserializeError: DeserializeJson<ErrorMessages>
        );

    private static ProgressReportingHttpContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        System.Console.WriteLine($"[DEBUG] Serializing request: {json}");
        return new ProgressReportingHttpContent(json, contentType: "application/json");
    }

    private static async Task<T> DeserializeJson<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        System.Console.WriteLine($"[DEBUG] Response status: {response.StatusCode}, URL: {response.RequestMessage?.RequestUri}, body: {body}");
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result ?? throw new InvalidOperationException($"Failed to deserialize response to type {typeof(T).Name}");
    }

    private static async Task<string> DeserializeString(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    ) =>
        await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

    private static async Task<string> DeserializeError(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return string.IsNullOrEmpty(content) ? "Unknown error" : content;
    }

    private static string BuildQueryString(params (string Key, object? Value)[] parameters)
    {
        var parts = new List<string>();
        foreach (var (key, value) in parameters)
        {
            if (value == null)
            {
                continue;
            }

            if (value is System.Collections.IEnumerable enumerable and not string)
            {
                foreach (var item in enumerable)
                {
                    if (item != null)
                    {
                        parts.Add($"{key}={Uri.EscapeDataString(item.ToString() ?? string.Empty)}");
                    }
                }
            }
            else
            {
                parts.Add($"{key}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}");
            }
        }
        return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
    }
}