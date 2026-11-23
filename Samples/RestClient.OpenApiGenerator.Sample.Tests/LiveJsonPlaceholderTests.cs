using Microsoft.Extensions.DependencyInjection;

namespace RestClient.OpenApiGenerator.Sample.Tests;

// Exhaustion handles this
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

/// <summary>Integration tests for JSONPlaceholder API using actual HTTP calls.</summary>
[TestClass]
public sealed class LiveJsonPlaceholderTests
{
    private static IHttpClientFactory _httpClientFactory = null!;

    [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
    public static void ClassInitialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var services = new ServiceCollection();
        _ = services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    }

    [TestMethod]
    public async Task GetTodos_ReturnsListOfTodos()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient.GetTodosAsync().ConfigureAwait(false);

        var todos = result switch
        {
            OkTodos(var value) => value,
            ErrorTodos(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorTodos(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(todos);
        Assert.IsTrue(todos.Count > 0);
        Assert.IsFalse(string.IsNullOrEmpty(todos[0].Title));
    }

    [TestMethod]
    public async Task CreateTodo_ReturnsCreatedTodo()
    {
        var newTodo = new TodoInput(UserId: 1, Title: "Test Todo", Completed: false);

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .CreateTodoAsync(newTodo, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var todo = result switch
        {
            OkTodo(var value) => value,
            ErrorTodo(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorTodo(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(todo);
        Assert.IsTrue(todo.Id > 0);
        Assert.AreEqual("Test Todo", todo.Title);
    }

    [TestMethod]
    public async Task UpdateTodo_ReturnsUpdatedTodo()
    {
        var updatedTodo = new TodoInput(UserId: 1, Title: "Updated Test Todo", Completed: true);

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .UpdateTodoAsync(1, updatedTodo, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var todo = result switch
        {
            OkTodo(var value) => value,
            ErrorTodo(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorTodo(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(todo);
        Assert.AreEqual("Updated Test Todo", todo.Title);
        Assert.IsTrue(todo.Completed);
    }

    [TestMethod]
    public async Task DeleteTodo_Succeeds()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .DeleteTodoAsync(id: 1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        Assert.IsTrue(result.IsOk);
    }

    [TestMethod]
    public async Task GetPosts_ReturnsListOfPosts()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .GetPostsAsync(cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var posts = result switch
        {
            OkPosts(var value) => value,
            ErrorPosts(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorPosts(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(posts);
        Assert.IsTrue(posts.Count > 0);
        Assert.IsFalse(string.IsNullOrEmpty(posts[0].Title));
    }

    [TestMethod]
    public async Task CreatePost_ReturnsCreatedPost()
    {
        var newPost = new PostInput(
            UserId: 1,
            Title: "Test Post",
            Body: "This is a test post body"
        );

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .CreatePostAsync(newPost, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var post = result switch
        {
            OkPost(var value) => value,
            ErrorPost(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorPost(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(post);
        Assert.IsTrue(post.Id > 0);
        Assert.AreEqual("Test Post", post.Title);
    }

    [TestMethod]
    public async Task UpdatePost_ReturnsUpdatedPost()
    {
        var updatedPost = new PostInput(
            UserId: 1,
            Title: "Updated Test Post",
            Body: "This is an updated test post body"
        );

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .UpdatePostAsync(1, updatedPost, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var post = result switch
        {
            OkPost(var value) => value,
            ErrorPost(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorPost(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(post);
        Assert.AreEqual("Updated Test Post", post.Title);
    }

    [TestMethod]
    public async Task DeletePost_Succeeds()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .DeletePostAsync(1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        Assert.IsTrue(result.IsOk);
    }

    [TestMethod]
    public async Task GetPostById_ReturnsPost()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .GetPostByIdAsync(1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var post = result switch
        {
            OkPost(var value) => value,
            ErrorPost(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorPost(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(post);
        Assert.AreEqual(1, post.Id);
        Assert.IsFalse(string.IsNullOrEmpty(post.Title));
    }

    [TestMethod]
    public async Task GetUserById_ReturnsUser()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .GetUserByIdAsync(1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        var user = result switch
        {
            OkUser(var value) => value,
            ErrorUser(ExceptionErrorString(var ex)) => throw new InvalidOperationException(
                "Expected success result",
                ex
            ),
            ErrorUser(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected success result but got error: HTTP {statusCode}: {body}"
                ),
        };

        Assert.IsNotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.IsFalse(string.IsNullOrEmpty(user.Name));
    }

    [TestMethod]
    public async Task GetTodos_WithCancelledToken_ReturnsErrorResult()
    {
        using var cts = new CancellationTokenSource();
#pragma warning disable CA1849 // Synchronous Cancel is acceptable for test purposes
        cts.Cancel();
#pragma warning restore CA1849

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .GetTodosAsync(cancellationToken: cts.Token)
            .ConfigureAwait(false);

        var exception = result switch
        {
            OkTodos(var value) => throw new InvalidOperationException(
                "Expected error result but got success"
            ),
            ErrorTodos(ExceptionErrorString(var ex)) => ex,
            ErrorTodos(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected exception error but got HTTP error: {statusCode}: {body}"
                ),
        };

        Assert.IsTrue(
            exception is OperationCanceledException or TaskCanceledException,
            $"Expected cancellation exception but got: {exception.GetType().Name}"
        );
    }

    [TestMethod]
    public async Task CreateTodo_WithCancelledToken_ReturnsErrorResult()
    {
        using var cts = new CancellationTokenSource();
#pragma warning disable CA1849 // Synchronous Cancel is acceptable for test purposes
        cts.Cancel();
#pragma warning restore CA1849

        var newTodo = new TodoInput(UserId: 1, Title: "Test Todo", Completed: false);

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .CreateTodoAsync(newTodo, cancellationToken: cts.Token)
            .ConfigureAwait(false);

        var exception = result switch
        {
            OkTodo(var value) => throw new InvalidOperationException(
                "Expected error result but got success"
            ),
            ErrorTodo(ExceptionErrorString(var ex)) => ex,
            ErrorTodo(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected exception error but got HTTP error: {statusCode}: {body}"
                ),
        };

        Assert.IsTrue(
            exception is OperationCanceledException or TaskCanceledException,
            $"Expected cancellation exception but got: {exception.GetType().Name}"
        );
    }

    [TestMethod]
    public async Task UpdateTodo_WithCancelledToken_ReturnsErrorResult()
    {
        using var cts = new CancellationTokenSource();
#pragma warning disable CA1849 // Synchronous Cancel is acceptable for test purposes
        cts.Cancel();
#pragma warning restore CA1849

        var updatedTodo = new TodoInput(UserId: 1, Title: "Updated Test Todo", Completed: true);

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .UpdateTodoAsync(1, updatedTodo, cancellationToken: cts.Token)
            .ConfigureAwait(false);

        var exception = result switch
        {
            OkTodo(var value) => throw new InvalidOperationException(
                "Expected error result but got success"
            ),
            ErrorTodo(ExceptionErrorString(var ex)) => ex,
            ErrorTodo(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected exception error but got HTTP error: {statusCode}: {body}"
                ),
        };

        Assert.IsTrue(
            exception is OperationCanceledException or TaskCanceledException,
            $"Expected cancellation exception but got: {exception.GetType().Name}"
        );
    }

    [TestMethod]
    public async Task DeleteTodo_WithCancelledToken_ReturnsErrorResult()
    {
        using var cts = new CancellationTokenSource();
#pragma warning disable CA1849 // Synchronous Cancel is acceptable for test purposes
        cts.Cancel();
#pragma warning restore CA1849

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .DeleteTodoAsync(1, cancellationToken: cts.Token)
            .ConfigureAwait(false);

        var exception = result switch
        {
            OkUnit(var value) => throw new InvalidOperationException(
                "Expected error result but got success"
            ),
            ErrorUnit(ExceptionErrorString(var ex)) => ex,
            ErrorUnit(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected exception error but got HTTP error: {statusCode}: {body}"
                ),
        };

        Assert.IsTrue(
            exception is OperationCanceledException or TaskCanceledException,
            $"Expected cancellation exception but got: {exception.GetType().Name}"
        );
    }

    [TestMethod]
    public async Task GetPostById_WithCancelledToken_ReturnsErrorResult()
    {
        using var cts = new CancellationTokenSource();
#pragma warning disable CA1849 // Synchronous Cancel is acceptable for test purposes
        cts.Cancel();
#pragma warning restore CA1849

        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .GetPostByIdAsync(1, cancellationToken: cts.Token)
            .ConfigureAwait(false);

        var exception = result switch
        {
            OkPost(var value) => throw new InvalidOperationException(
                "Expected error result but got success"
            ),
            ErrorPost(ExceptionErrorString(var ex)) => ex,
            ErrorPost(ResponseErrorString(var body, var statusCode, _)) =>
                throw new InvalidOperationException(
                    $"Expected exception error but got HTTP error: {statusCode}: {body}"
                ),
        };

        Assert.IsTrue(
            exception is OperationCanceledException or TaskCanceledException,
            $"Expected cancellation exception but got: {exception.GetType().Name}"
        );
    }
}
