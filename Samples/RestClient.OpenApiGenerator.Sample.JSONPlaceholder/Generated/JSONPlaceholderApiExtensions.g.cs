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

namespace JSONPlaceholder.Generated;

/// <summary>Extension methods for API operations.</summary>
public static class JSONPlaceholderApiExtensions
{
    #region Configuration

    private static readonly AbsoluteUrl BaseUrl = "https://jsonplaceholder.typicode.com".ToAbsoluteUrl();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
        Task.FromResult(Unit.Value);

    #endregion

    #region Posts Operations

    /// <summary>Get all posts</summary>
    public static Task<Result<List<Post>, HttpError<string>>> GetPostsAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _getPostsAsync(httpClient, Unit.Value, cancellationToken);
    
    /// <summary>Create a new post</summary>
    public static Task<Result<Post, HttpError<string>>> CreatePostAsync(
        this HttpClient httpClient,
        PostInput body,
        CancellationToken cancellationToken = default
    ) => _createPostAsync(httpClient, body, cancellationToken);
    
    /// <summary>Get a post by ID</summary>
    public static Task<Result<Post, HttpError<string>>> GetPostByIdAsync(
        this HttpClient httpClient,
        long id,
        CancellationToken cancellationToken = default
    ) => _getPostByIdAsync(httpClient, id, cancellationToken);
    
    /// <summary>Update a post</summary>
    public static Task<Result<Post, HttpError<string>>> UpdatePostAsync(
        this HttpClient httpClient,
        long id, PostInput body,
        CancellationToken cancellationToken = default
    ) => _updatePostAsync(httpClient, (id, body), cancellationToken);
    
    /// <summary>Delete a post</summary>
    public static Task<Result<Unit, HttpError<string>>> DeletePostAsync(
        this HttpClient httpClient,
        long id,
        CancellationToken cancellationToken = default
    ) => _deletePostAsync(httpClient, id, cancellationToken);

    #endregion

    #region Todos Operations

    /// <summary>Get all todos</summary>
    public static Task<Result<List<Todo>, HttpError<string>>> GetTodosAsync(
        this HttpClient httpClient,
        
        CancellationToken cancellationToken = default
    ) => _getTodosAsync(httpClient, Unit.Value, cancellationToken);
    
    /// <summary>Create a new todo</summary>
    public static Task<Result<Todo, HttpError<string>>> CreateTodoAsync(
        this HttpClient httpClient,
        TodoInput body,
        CancellationToken cancellationToken = default
    ) => _createTodoAsync(httpClient, body, cancellationToken);
    
    /// <summary>Get a todo by ID</summary>
    public static Task<Result<Todo, HttpError<string>>> GetTodoByIdAsync(
        this HttpClient httpClient,
        long id,
        CancellationToken cancellationToken = default
    ) => _getTodoByIdAsync(httpClient, id, cancellationToken);
    
    /// <summary>Update a todo</summary>
    public static Task<Result<Todo, HttpError<string>>> UpdateTodoAsync(
        this HttpClient httpClient,
        long id, TodoInput body,
        CancellationToken cancellationToken = default
    ) => _updateTodoAsync(httpClient, (id, body), cancellationToken);
    
    /// <summary>Delete a todo</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteTodoAsync(
        this HttpClient httpClient,
        long id,
        CancellationToken cancellationToken = default
    ) => _deleteTodoAsync(httpClient, id, cancellationToken);

    #endregion

    #region Users Operations

    /// <summary>Get a user by ID</summary>
    public static Task<Result<User, HttpError<string>>> GetUserByIdAsync(
        this HttpClient httpClient,
        long id,
        CancellationToken cancellationToken = default
    ) => _getUserByIdAsync(httpClient, id, cancellationToken);

    #endregion

    private static GetAsync<List<Post>, string, Unit> _getPostsAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<List<Post>, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/posts"), null, null),
            deserializeSuccess: DeserializeJson<List<Post>>,
            deserializeError: DeserializeError
        );

    private static PostAsync<Post, string, PostInput> _createPostAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<Post, string, PostInput>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/posts"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<Post>,
            deserializeError: DeserializeError
        );

    private static GetAsync<Post, string, long> _getPostByIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<Post, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/posts/{id}"), null, null),
            deserializeSuccess: DeserializeJson<Post>,
            deserializeError: DeserializeError
        );

    private static PutAsync<Post, string, (long Params, PostInput Body)> _updatePostAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<Post, string, (long Params, PostInput Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/posts/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<Post>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, long> _deletePostAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/posts/{id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<List<Todo>, string, Unit> _getTodosAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<List<Todo>, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/todos"), null, null),
            deserializeSuccess: DeserializeJson<List<Todo>>,
            deserializeError: DeserializeError
        );

    private static PostAsync<Todo, string, TodoInput> _createTodoAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<Todo, string, TodoInput>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/todos"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<Todo>,
            deserializeError: DeserializeError
        );

    private static GetAsync<Todo, string, long> _getTodoByIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<Todo, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/todos/{id}"), null, null),
            deserializeSuccess: DeserializeJson<Todo>,
            deserializeError: DeserializeError
        );

    private static PutAsync<Todo, string, (long Params, TodoInput Body)> _updateTodoAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<Todo, string, (long Params, TodoInput Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/todos/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<Todo>,
            deserializeError: DeserializeError
        );

    private static DeleteAsync<Unit, string, long> _deleteTodoAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/todos/{id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    private static GetAsync<User, string, long> _getUserByIdAsync { get; } =
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<User, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/users/{id}"), null, null),
            deserializeSuccess: DeserializeJson<User>,
            deserializeError: DeserializeError
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