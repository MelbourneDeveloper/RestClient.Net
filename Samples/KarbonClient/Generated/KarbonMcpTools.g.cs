#nullable enable
using System.ComponentModel;
using System.Text.Json;
using Outcome;
using Karbon.Generated;
using ModelContextProtocol.Server;

namespace Karbon.Mcp;

/// <summary>MCP server tools for Karbon API.</summary>
[McpServerToolType]
public static class KarbonTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Client Groups from your tenant. Using the query parameters available to this endpoint, you can also filter the list of Client Groups by their full name.  **Notes**  * This endpoint returns a maximum of 100 Client Groups at once. * If the query results in more than 100 Client Groups, a link to the next set of the results will be given in the `@odata.nextLink` field of the response. * The `$filter` query parameter supports a logical operator - `eq` and a property to help you form an expression.</summary>
    /// <param name="filter">When this parameter is combined with the URI, this endpoint will return a subset of the Client Groups that satisfy the `$filter` expression.</param>
    /// <param name="orderby">When this parameter is combined with the URI, this endpoint will return a list of Client Groups, sorted by the available properties.</param>
    /// <param name="top">Limit the number of items returned</param>
    /// <param name="skip">Skip the first n items</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `GET` method on this endpoint to receive a paginated list of Client Groups from your tenant. Using the query parameters available to this endpoint, you can also filter the list of Client Groups by their full name.  **Notes**  * This endpoint returns a maximum of 100 Client Groups at once. * If the query results in more than 100 Client Groups, a link to the next set of the results will be given in the `@odata.nextLink` field of the response. * The `$filter` query parameter supports a logical operator - `eq` and a property to help you form an expression.")]
    public static async Task<string> GetClientGroups(HttpClient httpClient, string? filter = null, string orderby = "ClientGroupKey", int? top = null, int? skip = null)
    {
        var result = await httpClient.GetClientGroupsAsync(filter ?? "", orderby, top, skip, CancellationToken.None);

        return result switch
        {
            OkGetClientGroupErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGetClientGroupErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `POST` method on this endpoint to create a new Client Group in your tenant.</summary>
    
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `POST` method on this endpoint to create a new Client Group in your tenant.")]
    public static async Task<string> CreateClientGroup(HttpClient httpClient, object body)
    {
        var result = await httpClient.CreateClientGroupAsync(body, CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `GET` method on this endpoint to receive the details of a Client Group specified using the UserDefinedIdentifier.  Using the query parameter available to this endpoint, you can also include Business Card details of the Client Group in the response.</summary>
    /// <param name="userDefinedIdentifier">A unique identifier that you had created to identify this Client Group. This parameter is **not** case sensitive.</param>
    /// <param name="expand">When this parameter is combined with the URI, this endpoint will also return the Business Card of the Client Group.</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `GET` method on this endpoint to receive the details of a Client Group specified using the UserDefinedIdentifier.  Using the query parameter available to this endpoint, you can also include Business Card details of the Client Group in the response.")]
    public static async Task<string> GetClientGroupByUDI(HttpClient httpClient, string userDefinedIdentifier, string? expand = null)
    {
        var result = await httpClient.GetClientGroupByUDIAsync(userDefinedIdentifier, expand ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `GET` method on this endpoint to receive the details of a Client Group specified using the `ClientGroupkey`.  Using the query parameter available to this endpoint,  you can also include the Business Card details of the Client Group in the response.</summary>
    /// <param name="clientGroupkey">The Karbon-generated Client Group key</param>
    /// <param name="expand">When this parameter is specified in the query string this endpoint will also return the Business Card and/or Client Team of the Client Group.</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `GET` method on this endpoint to receive the details of a Client Group specified using the `ClientGroupkey`.  Using the query parameter available to this endpoint,  you can also include the Business Card details of the Client Group in the response.")]
    public static async Task<string> GetClientGroupByID(HttpClient httpClient, string clientGroupkey, string? expand = null)
    {
        var result = await httpClient.GetClientGroupByIDAsync(clientGroupkey, expand ?? "", CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `PUT` method on this endpoint to update full details of a Client Group specified using the `ClientGroupkey`.  Using the query parameter available to this endpoint, you can also update the Business Card details of the Client Group.</summary>
    /// <param name="clientGroupkey">The Karbon-generated Client Group key</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `PUT` method on this endpoint to update full details of a Client Group specified using the `ClientGroupkey`.  Using the query parameter available to this endpoint, you can also update the Business Card details of the Client Group.")]
    public static async Task<string> PutClientGroupByID(HttpClient httpClient, string clientGroupkey, object body)
    {
        var result = await httpClient.PutClientGroupByIDAsync(clientGroupkey, body, CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `PATCH` method on this endpoint to update partial details of a Client Group specified using the `ClientGroupkey`.  This method **only supports** editing the `FullName` property.</summary>
    /// <param name="clientGroupkey">The Karbon-generated Client Group key</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `PATCH` method on this endpoint to update partial details of a Client Group specified using the `ClientGroupkey`.  This method **only supports** editing the `FullName` property.")]
    public static async Task<string> PatchClientGroupByID(HttpClient httpClient, string clientGroupkey, object body)
    {
        var result = await httpClient.PatchClientGroupByIDAsync(clientGroupkey, body, CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `GET` method on this endpoint to receive a paginated list of Contacts from your tenant.   Using the query parameters available to this endpoint,  you can also filter the list of Contacts by their full name, email address, or phone number.  **Notes**  <ul>  <li>This endpoint returns a maximum of 100 Contacts at once.</li>  <li>If the query results in more than 100 Contacts, a link to the next set of the results will be given in the `@odata.nextLink` field of the response.</li>  <li>The `$filter` query parameter supports 3 logical operators (`eq`, `contains`, and `and`) and 3 properties to help you form an expression. Usage examples below</li> <table> <thead> <tr> <th>Logical Operators</th> <th>Purpose</th> <th>FullName</th> <th>EmailAddress</th> <th>PhoneNumber</th> </tr> </thead> <tbody> <tr> <td>eq</td> <td>Full-text search</td> <td>/v3/Contacts?$filter=FullName eq 'Sample Management Team'</td> <td>/v3/Contacts?$filter=EmailAddress eq 'sample@company.com'</td> <td>/v3/Contacts?$filter=PhoneNumber eq '1234567890'</td> </tr> <tr> <td>contains</td> <td>Partial-text search</td> <td>/v3/Contacts?$filter=(contains(FullName, 'Management Team'))</td> <td>/v3/Contacts?$filter=(contains(EmailAddress, 'sample@'))</td> <td>/v3/Contacts?$filter=(contains(PhoneNumber, '45678'))</td> </tr> <tr> <td>and</td> <td>Combines properties</td> <td colspan = '2'>/v3/Contacts?$filter=PhoneNumber eq '1234567890' and (contains(FullName, 'Management Team'))</td> </tr> </tbody> </table></summary>
    /// <param name="filter">When this parameter is combined with the URI, this endpoint will return a subset of the Contacts that satisfy the `$filter` expression.</param>
    /// <param name="orderby">When this parameter is combined with the URI, this endpoint will return a list of Contacts, sorted by the available properties.</param>
    /// <param name="top">Limit the number of items returned</param>
    /// <param name="skip">Skip the first n items</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `GET` method on this endpoint to receive a paginated list of Contacts from your tenant.   Using the query parameters available to this endpoint,  you can also filter the list of Contacts by their full name, email address, or phone number.  **Notes**  <ul>  <li>This endpoint returns a maximum of 100 Contacts at once.</li>  <li>If the query results in more than 100 Contacts, a link to the next set of the results will be given in the `@odata.nextLink` field of the response.</li>  <li>The `$filter` query parameter supports 3 logical operators (`eq`, `contains`, and `and`) and 3 properties to help you form an expression. Usage examples below</li> <table> <thead> <tr> <th>Logical Operators</th> <th>Purpose</th> <th>FullName</th> <th>EmailAddress</th> <th>PhoneNumber</th> </tr> </thead> <tbody> <tr> <td>eq</td> <td>Full-text search</td> <td>/v3/Contacts?$filter=FullName eq 'Sample Management Team'</td> <td>/v3/Contacts?$filter=EmailAddress eq 'sample@company.com'</td> <td>/v3/Contacts?$filter=PhoneNumber eq '1234567890'</td> </tr> <tr> <td>contains</td> <td>Partial-text search</td> <td>/v3/Contacts?$filter=(contains(FullName, 'Management Team'))</td> <td>/v3/Contacts?$filter=(contains(EmailAddress, 'sample@'))</td> <td>/v3/Contacts?$filter=(contains(PhoneNumber, '45678'))</td> </tr> <tr> <td>and</td> <td>Combines properties</td> <td colspan = '2'>/v3/Contacts?$filter=PhoneNumber eq '1234567890' and (contains(FullName, 'Management Team'))</td> </tr> </tbody> </table>")]
    public static async Task<string> GetAllContacts(HttpClient httpClient, string? filter = null, string orderby = "ClientGroupKey", int? top = null, int? skip = null)
    {
        var result = await httpClient.GetAllContactsAsync(filter ?? "", orderby, top, skip, CancellationToken.None);

        return result switch
        {
            OkGetContactsErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorGetContactsErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `POST` method on this endpoint to create a new Contact in your tenant.</summary>
    
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `POST` method on this endpoint to create a new Contact in your tenant.")]
    public static async Task<string> CreateContact(HttpClient httpClient, CreateContact body)
    {
        var result = await httpClient.CreateContactAsync(body, CancellationToken.None);

        return result switch
        {
            OkResponseCreateContactErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResponseCreateContactErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `GET` method on this endpoint to receive the details of a Contact specified using the UserDefinedIdentifier.  Using the query parameter available to this endpoint, you can also include Business Card details of the Contact in the response.</summary>
    /// <param name="userDefinedIdentifier">A unique identifier that you had created to identify this Contact. This parameter is **not** case sensitive.</param>
    /// <param name="expand">When this parameter is combined with the URI, this endpoint will also return the Business Cards of the Contact.</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `GET` method on this endpoint to receive the details of a Contact specified using the UserDefinedIdentifier.  Using the query parameter available to this endpoint, you can also include Business Card details of the Contact in the response.")]
    public static async Task<string> GetContactByUDI(HttpClient httpClient, string userDefinedIdentifier, string? expand = null)
    {
        var result = await httpClient.GetContactByUDIAsync(userDefinedIdentifier, expand ?? "", CancellationToken.None);

        return result switch
        {
            OkResponseCreateContactErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResponseCreateContactErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `GET` method on this endpoint to receive the details of a Contact specified using the `Contactkey`.  Using the query parameter available to this endpoint,  you can also include the Business Card details of the Contacts in the response.</summary>
    /// <param name="contactkey">The Karbon-generated Contact key</param>
    /// <param name="expand">When this parameter is combined with the URI, this endpoint will also return Business Card and/or the Client Team of the Contact.</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `GET` method on this endpoint to receive the details of a Contact specified using the `Contactkey`.  Using the query parameter available to this endpoint,  you can also include the Business Card details of the Contacts in the response.")]
    public static async Task<string> GetContactByID(HttpClient httpClient, string contactkey, string? expand = null)
    {
        var result = await httpClient.GetContactByIDAsync(contactkey, expand ?? "", CancellationToken.None);

        return result switch
        {
            OkResponseCreateContactErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorResponseCreateContactErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `PUT` method on this endpoint to update full details of a Contact specified using the `Contactkey`.  Using the query parameter `$expand` with this endpoint, you can also update the Business Card details of the Contact.  **Note:** When updating business cards, you must include `BusinessCardKey`, `EntityType` and `EntityKey` properties for existing Business Cards - or the existing card and any associated Client Requests may be removed.</summary>
    /// <param name="contactkey">The Karbon-generated Contact key</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `PUT` method on this endpoint to update full details of a Contact specified using the `Contactkey`.  Using the query parameter `$expand` with this endpoint, you can also update the Business Card details of the Contact.  **Note:** When updating business cards, you must include `BusinessCardKey`, `EntityType` and `EntityKey` properties for existing Business Cards - or the existing card and any associated Client Requests may be removed.")]
    public static async Task<string> PutContactByID(HttpClient httpClient, string contactkey, CreateContact body)
    {
        var result = await httpClient.PutContactByIDAsync(contactkey, body, CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }

    /// <summary>Use the `PATCH` method on this endpoint to update partial details of a Contact specified using the `Contactkey`.  This method **only supports** editing the `FirstName`, `MiddleName`, `LastName`, `PreferredName`, `Salutation` and `Suffix` properties.</summary>
    /// <param name="contactkey">The Karbon-generated Contact key</param>
    /// <param name="body">Request body</param>
    /// <param name="httpClient">HttpClient instance</param>
    [McpServerTool, Description("Use the `PATCH` method on this endpoint to update partial details of a Contact specified using the `Contactkey`.  This method **only supports** editing the `FirstName`, `MiddleName`, `LastName`, `PreferredName`, `Salutation` and `Suffix` properties.")]
    public static async Task<string> PatchContactsByID(HttpClient httpClient, string contactkey, object body)
    {
        var result = await httpClient.PatchContactsByIDAsync(contactkey, body, CancellationToken.None);

        return result switch
        {
            OkobjectErrorMessages(var success) =>
                JsonSerializer.Serialize(success, JsonOptions),
            ErrorobjectErrorMessages(var httpError) => httpError switch
            {
                HttpError<ErrorMessages>.ErrorResponseError err =>
                    $"Error {err.StatusCode}: {JsonSerializer.Serialize(err.Body, JsonOptions)}",
                HttpError<ErrorMessages>.ExceptionError err =>
                    $"Exception: {err.Exception.Message}",
                _ => "Unknown error"
            },
            _ => "Unknown result"
        };
    }
}