using Microsoft.Extensions.DependencyInjection;
using Outcome;

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
        AssertSuccessResult(result, todos);

        Assert.IsNotNull(todos);
        Assert.IsTrue(todos.Count > 0);
        Assert.IsFalse(string.IsNullOrEmpty(todos[0].Title));
        Assert.AreEqual(todos.Count, todos.Select(todo => todo.Id).Distinct().Count());
        Assert.IsTrue(
            todos.All(todo =>
                todo.Id > 0 && todo.UserId > 0 && !string.IsNullOrWhiteSpace(todo.Title)
            )
        );
        Assert.AreEqual(1L, todos[0].Id);
        Assert.AreEqual(1L, todos[0].UserId);
        var completed = todos.Where(todo => todo.Completed).ToArray();
        var incomplete = todos.Where(todo => !todo.Completed).ToArray();
        Assert.IsTrue(completed.Length > 0);
        Assert.IsTrue(incomplete.Length > 0);
        Assert.AreEqual(todos.Count, completed.Length + incomplete.Length);
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
        AssertSuccessResult(result, todo);

        Assert.IsNotNull(todo);
        Assert.IsTrue(todo.Id > 0);
        Assert.AreEqual("Test Todo", todo.Title);
        Assert.AreEqual(newTodo.UserId, todo.UserId);
        Assert.AreEqual(newTodo.Completed, todo.Completed);
        Assert.IsFalse(todo.Completed);
        Assert.AreEqual(new TodoInput(1, "Test Todo", false), newTodo);
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
        AssertSuccessResult(result, todo);

        Assert.IsNotNull(todo);
        Assert.AreEqual("Updated Test Todo", todo.Title);
        Assert.IsTrue(todo.Completed);
        Assert.AreEqual(1L, todo.Id);
        Assert.AreEqual(updatedTodo.UserId, todo.UserId);
        Assert.AreEqual(updatedTodo.Completed, todo.Completed);
        Assert.AreEqual(new TodoInput(1, "Updated Test Todo", true), updatedTodo);
    }

    [TestMethod]
    public async Task DeleteTodo_Succeeds()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .DeleteTodoAsync(id: 1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        AssertSuccessResult(result, Unit.Value);
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
        AssertSuccessResult(result, posts);

        Assert.IsNotNull(posts);
        Assert.IsTrue(posts.Count > 0);
        Assert.IsFalse(string.IsNullOrEmpty(posts[0].Title));
        Assert.AreEqual(posts.Count, posts.Select(post => post.Id).Distinct().Count());
        Assert.IsTrue(
            posts.All(post =>
                post.Id > 0
                && post.UserId > 0
                && !string.IsNullOrWhiteSpace(post.Title)
                && !string.IsNullOrWhiteSpace(post.Body)
            )
        );
        var firstResult = await httpClient
            .GetPostByIdAsync(posts[0].Id, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);
        var firstPost = ((OkPost)firstResult).Value;
        AssertSuccessResult(firstResult, firstPost);
        Assert.AreEqual(
            posts[0],
            firstPost,
            "List and individual-resource endpoints must deserialize the same post."
        );
        Assert.AreNotSame(posts[0], firstPost);
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
        AssertSuccessResult(result, post);

        Assert.IsNotNull(post);
        Assert.IsTrue(post.Id > 0);
        Assert.AreEqual("Test Post", post.Title);
        Assert.AreEqual(newPost.UserId, post.UserId);
        Assert.AreEqual(newPost.Body, post.Body);
        Assert.AreEqual(new PostInput(1, "Test Post", "This is a test post body"), newPost);
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
        AssertSuccessResult(result, post);

        Assert.IsNotNull(post);
        Assert.AreEqual("Updated Test Post", post.Title);
        Assert.AreEqual(1L, post.Id);
        Assert.AreEqual(updatedPost.UserId, post.UserId);
        Assert.AreEqual(updatedPost.Body, post.Body);
        Assert.AreEqual(
            new PostInput(1, "Updated Test Post", "This is an updated test post body"),
            updatedPost
        );
    }

    [TestMethod]
    public async Task DeletePost_Succeeds()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient
            .DeletePostAsync(1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);

        AssertSuccessResult(result, Unit.Value);
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
        AssertSuccessResult(result, post);

        Assert.IsNotNull(post);
        Assert.AreEqual(1, post.Id);
        Assert.IsFalse(string.IsNullOrEmpty(post.Title));
        Assert.AreEqual(1L, post.UserId);
        Assert.AreEqual(
            "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
            post.Title
        );
        Assert.AreEqual(
            "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto",
            post.Body
        );
        var repeatedResult = await httpClient
            .GetPostByIdAsync(1, cancellationToken: CancellationToken.None)
            .ConfigureAwait(false);
        var repeated = ((OkPost)repeatedResult).Value;
        AssertSuccessResult(repeatedResult, repeated);
        Assert.AreEqual(post, repeated);
        Assert.AreNotSame(post, repeated);
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
        AssertSuccessResult(result, user);

        Assert.IsNotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.IsFalse(string.IsNullOrEmpty(user.Name));
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Username));
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Email));
        StringAssert.Contains(user.Email, "@");
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Phone));
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Website));
        Assert.IsNotNull(user.Address);
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Address.Street));
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Address.City));
        Assert.IsNotNull(user.Address.Geo);
        Assert.IsTrue(
            double.TryParse(
                user.Address.Geo.Lat,
                System.Globalization.CultureInfo.InvariantCulture,
                out var latitude
            )
        );
        Assert.IsTrue(
            double.TryParse(
                user.Address.Geo.Lng,
                System.Globalization.CultureInfo.InvariantCulture,
                out var longitude
            )
        );
        Assert.IsTrue(latitude is >= -90 and <= 90);
        Assert.IsTrue(longitude is >= -180 and <= 180);
        Assert.IsNotNull(user.Company);
        Assert.IsFalse(string.IsNullOrWhiteSpace(user.Company.Name));
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
        AssertCancelledResult(result, exception, cts.Token);

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
        AssertCancelledResult(result, exception, cts.Token);

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
        AssertCancelledResult(result, exception, cts.Token);

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
        AssertCancelledResult(result, exception, cts.Token);

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
        AssertCancelledResult(result, exception, cts.Token);

        Assert.IsTrue(
            exception is OperationCanceledException or TaskCanceledException,
            $"Expected cancellation exception but got: {exception.GetType().Name}"
        );
    }

    private static void AssertSuccessResult<T>(Result<T, HttpError<string>> result, T expected)
    {
        Assert.IsTrue(result.IsOk);
        Assert.IsFalse(result.IsError);
        Assert.IsInstanceOfType<Result<T, HttpError<string>>.Ok<T, HttpError<string>>>(result);
        Assert.AreEqual(
            expected,
            ((Result<T, HttpError<string>>.Ok<T, HttpError<string>>)result).Value
        );
        var successCalls = 0;
        var errorCalls = 0;
        var tapped = result.Tap(
            value =>
            {
                successCalls++;
                Assert.AreEqual(expected, value);
            },
            _ =>
            {
                errorCalls++;
            }
        );
        Assert.AreSame(result, tapped);
        Assert.AreEqual(1, successCalls);
        Assert.AreEqual(0, errorCalls);
        var branch = result.Match(
            value =>
            {
                successCalls++;
                Assert.AreEqual(expected, value);
                return "success";
            },
            _ =>
            {
                errorCalls++;
                return "error";
            }
        );
        Assert.AreEqual("success", branch);
        Assert.AreEqual(2, successCalls);
        Assert.AreEqual(0, errorCalls);
        var bound = result.Bind(value =>
        {
            successCalls++;
            Assert.AreEqual(expected, value);
            return result;
        });
        Assert.AreSame(result, bound);
        Assert.AreEqual(3, successCalls);
        Assert.AreEqual(0, errorCalls);
        Assert.AreEqual(expected, result.GetValueOrDefault(default(T)!));
    }

    private static void AssertCancelledResult<T>(
        Result<T, HttpError<string>> result,
        Exception expected,
        CancellationToken token
    )
    {
        Assert.IsTrue(token.IsCancellationRequested);
        Assert.IsTrue(result.IsError);
        Assert.IsFalse(result.IsOk);
        Assert.IsInstanceOfType<OperationCanceledException>(expected);
        var error = ((Result<T, HttpError<string>>.Error<T, HttpError<string>>)result).Value;
        Assert.IsInstanceOfType<HttpError<string>.ExceptionError>(error);
        Assert.AreSame(expected, ((HttpError<string>.ExceptionError)error).Exception);
        var successCalls = 0;
        var errorCalls = 0;
        var tapped = result.Tap(
            _ =>
            {
                successCalls++;
            },
            actual =>
            {
                errorCalls++;
                Assert.AreSame(error, actual);
            }
        );
        Assert.AreSame(result, tapped);
        Assert.AreEqual(0, successCalls);
        Assert.AreEqual(1, errorCalls);
        var branch = result.Match(
            _ =>
            {
                successCalls++;
                return "success";
            },
            actual =>
            {
                errorCalls++;
                Assert.AreSame(error, actual);
                return "cancelled";
            }
        );
        Assert.AreEqual("cancelled", branch);
        Assert.AreEqual(0, successCalls);
        Assert.AreEqual(2, errorCalls);
        var mapped = result.Map(value =>
        {
            successCalls++;
            return value;
        });
        Assert.IsTrue(mapped.IsError);
        Assert.IsFalse(mapped.IsOk);
        Assert.AreSame(
            error,
            ((Result<T, HttpError<string>>.Error<T, HttpError<string>>)mapped).Value
        );
        Assert.AreEqual(0, successCalls);
        var mappedError = result.MapError(actual =>
        {
            errorCalls++;
            Assert.AreSame(error, actual);
            return actual;
        });
        Assert.IsTrue(mappedError.IsError);
        var preservedError = (
            (Result<T, HttpError<string>>.Error<T, HttpError<string>>)mappedError
        ).Value;
        Assert.AreSame(error, preservedError);
        Assert.AreSame(expected, ((HttpError<string>.ExceptionError)preservedError).Exception);
        Assert.AreEqual(3, errorCalls);
    }
}
