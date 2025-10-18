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

    #endregion

    #region Posts Operations

    /// <summary>Get all posts</summary>
    public static Task<Result<List<Post>, HttpError<string>>> GetPosts(
        this HttpClient httpClient,
        CancellationToken ct = default
    ) => _getPosts()(httpClient, Unit.Value, ct);
    
    /// <summary>Create a new post</summary>
    public static Task<Result<Post, HttpError<string>>> CreatePost(
        this HttpClient httpClient,
        PostInput body,
        CancellationToken ct = default
    ) => _createPost()(httpClient, body, ct);
    
    /// <summary>Get a post by ID</summary>
    public static Task<Result<Post, HttpError<string>>> GetPostById(
        this HttpClient httpClient,
        long id,
        CancellationToken ct = default
    ) => _getPostById()(httpClient, id, ct);
    
    /// <summary>Update a post</summary>
    public static Task<Result<Post, HttpError<string>>> UpdatePost(
        this HttpClient httpClient,
        (long Params, PostInput Body) param,
        CancellationToken ct = default
    ) => _updatePost()(httpClient, param, ct);
    
    /// <summary>Delete a post</summary>
    public static Task<Result<Unit, HttpError<string>>> DeletePost(
        this HttpClient httpClient,
        long id,
        CancellationToken ct = default
    ) => _deletePost()(httpClient, id, ct);

    #endregion

    #region Todos Operations

    /// <summary>Get all todos</summary>
    public static Task<Result<List<Todo>, HttpError<string>>> GetTodos(
        this HttpClient httpClient,
        CancellationToken ct = default
    ) => _getTodos()(httpClient, Unit.Value, ct);
    
    /// <summary>Create a new todo</summary>
    public static Task<Result<Todo, HttpError<string>>> CreateTodo(
        this HttpClient httpClient,
        TodoInput body,
        CancellationToken ct = default
    ) => _createTodo()(httpClient, body, ct);
    
    /// <summary>Get a todo by ID</summary>
    public static Task<Result<Todo, HttpError<string>>> GetTodoById(
        this HttpClient httpClient,
        long id,
        CancellationToken ct = default
    ) => _getTodoById()(httpClient, id, ct);
    
    /// <summary>Update a todo</summary>
    public static Task<Result<Todo, HttpError<string>>> UpdateTodo(
        this HttpClient httpClient,
        (long Params, TodoInput Body) param,
        CancellationToken ct = default
    ) => _updateTodo()(httpClient, param, ct);
    
    /// <summary>Delete a todo</summary>
    public static Task<Result<Unit, HttpError<string>>> DeleteTodo(
        this HttpClient httpClient,
        long id,
        CancellationToken ct = default
    ) => _deleteTodo()(httpClient, id, ct);

    #endregion

    #region Users Operations

    /// <summary>Get a user by ID</summary>
    public static Task<Result<User, HttpError<string>>> GetUserById(
        this HttpClient httpClient,
        long id,
        CancellationToken ct = default
    ) => _getUserById()(httpClient, id, ct);

    #endregion

    private static readonly Deserialize<Unit> _deserializeUnit = static (_, _) =>
        Task.FromResult(Unit.Value);

    #region Posts Operations

    private static GetAsync<List<Post>, string, Unit> _getPosts() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<List<Post>, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/posts"), null, null),
            deserializeSuccess: DeserializeJson<List<Post>>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<Post, string, PostInput> _createPost() =>
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<Post, string, PostInput>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/posts"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<Post>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<Post, string, long> _getPostById() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<Post, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/posts/{id}"), null, null),
            deserializeSuccess: DeserializeJson<Post>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<Post, string, (long Params, PostInput Body)> _updatePost() =>
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<Post, string, (long Params, PostInput Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/posts/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<Post>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, long> _deletePost() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/posts/{id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    #endregion

    #region Todos Operations

    private static GetAsync<List<Todo>, string, Unit> _getTodos() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<List<Todo>, string, Unit>(
            url: BaseUrl,
            buildRequest: static _ => new HttpRequestParts(new RelativeUrl("/todos"), null, null),
            deserializeSuccess: DeserializeJson<List<Todo>>,
            deserializeError: DeserializeError
        );
    
    private static PostAsync<Todo, string, TodoInput> _createTodo() =>
        RestClient.Net.HttpClientFactoryExtensions.CreatePost<Todo, string, TodoInput>(
            url: BaseUrl,
            buildRequest: static body => new HttpRequestParts(new RelativeUrl("/todos"), CreateJsonContent(body), null),
            deserializeSuccess: DeserializeJson<Todo>,
            deserializeError: DeserializeError
        );
    
    private static GetAsync<Todo, string, long> _getTodoById() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<Todo, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/todos/{id}"), null, null),
            deserializeSuccess: DeserializeJson<Todo>,
            deserializeError: DeserializeError
        );
    
    private static PutAsync<Todo, string, (long Params, TodoInput Body)> _updateTodo() =>
        RestClient.Net.HttpClientFactoryExtensions.CreatePut<Todo, string, (long Params, TodoInput Body)>(
            url: BaseUrl,
            buildRequest: static param => new HttpRequestParts(new RelativeUrl($"/todos/{param.Params}"), CreateJsonContent(param.Body), null),
            deserializeSuccess: DeserializeJson<Todo>,
            deserializeError: DeserializeError
        );
    
    private static DeleteAsync<Unit, string, long> _deleteTodo() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateDelete<Unit, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/todos/{id}"), null, null),
            deserializeSuccess: _deserializeUnit,
            deserializeError: DeserializeError
        );

    #endregion

    #region Users Operations

    private static GetAsync<User, string, long> _getUserById() =>
        RestClient.Net.HttpClientFactoryExtensions.CreateGet<User, string, long>(
            url: BaseUrl,
            buildRequest: static id => new HttpRequestParts(new RelativeUrl($"/users/{id}"), null, null),
            deserializeSuccess: DeserializeJson<User>,
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