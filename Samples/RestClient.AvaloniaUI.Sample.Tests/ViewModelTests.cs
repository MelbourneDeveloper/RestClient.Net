using System.Net;
using System.Text;
using System.Text.Json;
using JSONPlaceholder.Generated;
using RestClient.AvaloniaUI.Sample.ViewModels;

namespace RestClient.AvaloniaUI.Sample.Tests;

/// <summary>View model tests for MainWindowViewModel using recorded HTTP interactions.</summary>
[TestClass]
public sealed class MainWindowViewModelTests
{
    [TestMethod]
    public async Task LoadPostsAsync_SuccessResponse_LoadsPostsIntoCollection()
    {
        using var firstResponse = GetPostsResponse();
        using var refreshedResponse = GetPostsResponse();
        using var factory = new RecordingFactory(() => firstResponse, () => refreshedResponse);
        var viewModel = new MainWindowViewModel(factory);
        AssertInitialState(viewModel, factory);

        factory.ObservedViewModel = viewModel;
        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        AssertLoadedPosts(viewModel);
        AssertReady(viewModel, factory, 1);
        AssertRequest(factory, 0, HttpMethod.Get, "/posts");
        var firstCollection = viewModel.Posts;
        firstCollection.Add(new Post(9, 999, "Locally stale", "Replace on refresh"));
        Assert.AreEqual(3, viewModel.Posts.Count);

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        AssertLoadedPosts(viewModel);
        Assert.AreNotSame(
            firstCollection,
            viewModel.Posts,
            "Refreshing must replace stale collection contents."
        );
        Assert.AreEqual(
            3,
            firstCollection.Count,
            "Refreshing must not mutate the caller's former collection."
        );
        AssertReady(viewModel, factory, 2);
        AssertRequest(factory, 1, HttpMethod.Get, "/posts");
    }

    [TestMethod]
    public async Task LoadPostsAsync_ErrorResponse_SetsErrorStatus()
    {
        using var failedResponse = GetErrorResponse();
        using var recoveredResponse = GetPostsResponse();
        using var secondFailure = GetErrorResponse();
        using var factory = new RecordingFactory(
            () => failedResponse,
            () => recoveredResponse,
            () => secondFailure
        );
        var viewModel = new MainWindowViewModel(factory);
        AssertInitialState(viewModel, factory);

        factory.ObservedViewModel = viewModel;
        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        AssertHttpError(viewModel, "loading");
        AssertReady(viewModel, factory, 1);
        AssertRequest(factory, 0, HttpMethod.Get, "/posts");

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        AssertLoadedPosts(viewModel);
        AssertReady(viewModel, factory, 2);
        AssertRequest(factory, 1, HttpMethod.Get, "/posts");
        var recoveredCollection = viewModel.Posts;

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreSame(
            recoveredCollection,
            viewModel.Posts,
            "A later load failure must preserve visible posts."
        );
        AssertPost(viewModel.Posts[0], 1, "sunt aut facere", "quia et suscipit");
        Assert.AreEqual(2, viewModel.Posts.Count);
        AssertHttpError(viewModel, "loading");
        AssertReady(viewModel, factory, 3);
        AssertRequest(factory, 2, HttpMethod.Get, "/posts");
    }

    [TestMethod]
    public async Task LoadPostsAsync_ExceptionThrown_SetsErrorStatus()
    {
        using var recoveredResponse = GetPostsResponse();
        using var refreshedResponse = GetPostsResponse();
        using var factory = new RecordingFactory(
            () => throw new HttpRequestException("Network error"),
            () => recoveredResponse,
            () => refreshedResponse
        );
        var viewModel = new MainWindowViewModel(factory);
        AssertInitialState(viewModel, factory);

        factory.ObservedViewModel = viewModel;
        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual("✗ Error loading posts: Network error", viewModel.StatusMessage);
        AssertReady(viewModel, factory, 1);
        AssertRequest(factory, 0, HttpMethod.Get, "/posts");

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        AssertLoadedPosts(viewModel);
        Assert.IsFalse(viewModel.StatusMessage.Contains("Network error", StringComparison.Ordinal));
        AssertReady(viewModel, factory, 2);
        AssertRequest(factory, 1, HttpMethod.Get, "/posts");

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);
        AssertLoadedPosts(viewModel);
        AssertReady(viewModel, factory, 3);
        AssertRequest(factory, 2, HttpMethod.Get, "/posts");
    }

    [TestMethod]
    public async Task CreatePostAsync_WithValidInput_CreatesAndAddsPost()
    {
        using var createdResponse = CreatePostResponse();
        using var secondResponse = CreatePostResponse();
        using var factory = new RecordingFactory(() => createdResponse, () => secondResponse);
        var viewModel = new MainWindowViewModel(factory)
        {
            NewPostTitle = "New Post",
            NewPostBody = "Post body",
        };
        AssertReady(viewModel, factory, 0);

        factory.ObservedViewModel = viewModel;
        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(1, viewModel.Posts.Count);
        AssertCreatedPost(viewModel);
        AssertRequest(factory, 0, HttpMethod.Post, "/posts", "New Post", "Post body");
        AssertReady(viewModel, factory, 1);
        var firstPost = viewModel.Posts[0];

        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please enter post title and body", viewModel.StatusMessage);
        Assert.AreEqual(1, viewModel.Posts.Count);
        Assert.AreSame(
            firstPost,
            viewModel.Posts[0],
            "Invalid submission must preserve the previously created post."
        );
        AssertReady(viewModel, factory, 1);

        viewModel.NewPostTitle = "New Post";
        viewModel.NewPostBody = "Post body";
        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(2, viewModel.Posts.Count);
        AssertCreatedPost(viewModel);
        Assert.AreSame(
            firstPost,
            viewModel.Posts[1],
            "A subsequent creation must insert ahead of existing posts."
        );
        AssertRequest(factory, 1, HttpMethod.Post, "/posts", "New Post", "Post body");
        AssertReady(viewModel, factory, 2);
    }

    [TestMethod]
    public async Task CreatePostAsync_WithEmptyTitle_SetsErrorStatus()
    {
        using var response = CreatePostResponse();
        using var factory = new RecordingFactory(() => response);
        var viewModel = new MainWindowViewModel(factory)
        {
            NewPostTitle = string.Empty,
            NewPostBody = "Post body",
        };

        factory.ObservedViewModel = viewModel;
        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual("✗ Please enter post title and body", viewModel.StatusMessage);
        Assert.AreEqual(string.Empty, viewModel.NewPostTitle);
        Assert.AreEqual(
            "Post body",
            viewModel.NewPostBody,
            "Validation must retain the typed body."
        );
        AssertReady(viewModel, factory, 0);

        viewModel.NewPostTitle = "New Post";
        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(1, viewModel.Posts.Count);
        AssertCreatedPost(viewModel);
        AssertRequest(factory, 0, HttpMethod.Post, "/posts", "New Post", "Post body");
        AssertReady(viewModel, factory, 1);

        viewModel.NewPostTitle = "Unsubmitted draft";
        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please enter post title and body", viewModel.StatusMessage);
        Assert.AreEqual(
            "Unsubmitted draft",
            viewModel.NewPostTitle,
            "Missing-body validation must retain the draft title."
        );
        Assert.AreEqual(string.Empty, viewModel.NewPostBody);
        Assert.AreEqual(1, viewModel.Posts.Count);
        AssertReady(viewModel, factory, 1);
    }

    [TestMethod]
    public async Task CreatePostAsync_ErrorResponse_SetsErrorStatus()
    {
        using var failedResponse = GetErrorResponse();
        using var createdResponse = CreatePostResponse();
        using var deletedResponse = DeletePostResponse();
        using var factory = new RecordingFactory(
            () => failedResponse,
            () => createdResponse,
            () => deletedResponse
        );
        var viewModel = new MainWindowViewModel(factory)
        {
            NewPostTitle = "New Post",
            NewPostBody = "Post body",
        };

        factory.ObservedViewModel = viewModel;
        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        AssertHttpError(viewModel, "creating");
        Assert.AreEqual(
            "New Post",
            viewModel.NewPostTitle,
            "Failed creation must preserve the draft for retry."
        );
        Assert.AreEqual("Post body", viewModel.NewPostBody);
        AssertRequest(factory, 0, HttpMethod.Post, "/posts", "New Post", "Post body");
        AssertReady(viewModel, factory, 1);

        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        AssertCreatedPost(viewModel);
        Assert.AreEqual(1, viewModel.Posts.Count);
        AssertRequest(factory, 1, HttpMethod.Post, "/posts", "New Post", "Post body");
        AssertReady(viewModel, factory, 2);
        var created = viewModel.Posts[0];

        await viewModel.DeletePostCommand.ExecuteAsync(created).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual("✓ Deleted post: New Post", viewModel.StatusMessage);
        AssertRequest(factory, 2, HttpMethod.Delete, "/posts/101");
        AssertReady(viewModel, factory, 3);
    }

    [TestMethod]
    public async Task UpdatePostAsync_WithNullPost_SetsErrorStatus()
    {
        using var updatedResponse = UpdatePostResponse();
        using var deletedResponse = DeletePostResponse();
        using var factory = new RecordingFactory(() => updatedResponse, () => deletedResponse);
        var viewModel = new MainWindowViewModel(factory);
        AssertInitialState(viewModel, factory);

        factory.ObservedViewModel = viewModel;
        await viewModel.UpdatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please select a post to update", viewModel.StatusMessage);
        Assert.AreEqual(0, viewModel.Posts.Count);
        AssertReady(viewModel, factory, 0);

        var original = new Post(1, 1, "Test", "Body");
        viewModel.Posts.Add(original);
        viewModel.SelectedPost = original;
        await viewModel.UpdatePostCommand.ExecuteAsync(original).ConfigureAwait(false);
        Assert.AreEqual(1, viewModel.Posts.Count);
        AssertPost(viewModel.Posts[0], 1, "Updated Post", "Updated body");
        Assert.AreNotSame(
            original,
            viewModel.Posts[0],
            "A successful update must replace the displayed post."
        );
        Assert.AreEqual("✓ Updated post: Updated Post", viewModel.StatusMessage);
        AssertRequest(factory, 0, HttpMethod.Put, "/posts/1", "Test [Updated]", "Body");
        AssertReady(viewModel, factory, 1);

        await viewModel.DeletePostCommand.ExecuteAsync(viewModel.Posts[0]).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual("✓ Deleted post: Updated Post", viewModel.StatusMessage);
        AssertRequest(factory, 1, HttpMethod.Delete, "/posts/1");
        AssertReady(viewModel, factory, 2);
    }

    [TestMethod]
    public async Task UpdatePostAsync_ErrorResponse_SetsErrorStatus()
    {
        using var failedResponse = GetErrorResponse();
        using var recoveredResponse = UpdatePostResponse();
        using var factory = new RecordingFactory(() => failedResponse, () => recoveredResponse);
        var viewModel = new MainWindowViewModel(factory);
        var original = new Post(1, 1, "Test", "Body");
        viewModel.Posts.Add(original);

        factory.ObservedViewModel = viewModel;
        await viewModel.UpdatePostCommand.ExecuteAsync(original).ConfigureAwait(false);
        AssertHttpError(viewModel, "updating");
        Assert.AreEqual(1, viewModel.Posts.Count);
        Assert.AreSame(
            original,
            viewModel.Posts[0],
            "A failed update must preserve the existing post."
        );
        AssertPost(original, 1, "Test", "Body");
        AssertRequest(factory, 0, HttpMethod.Put, "/posts/1", "Test [Updated]", "Body");
        AssertReady(viewModel, factory, 1);

        await viewModel.UpdatePostCommand.ExecuteAsync(original).ConfigureAwait(false);
        Assert.AreEqual(1, viewModel.Posts.Count);
        AssertPost(viewModel.Posts[0], 1, "Updated Post", "Updated body");
        Assert.AreEqual("✓ Updated post: Updated Post", viewModel.StatusMessage);
        AssertRequest(factory, 1, HttpMethod.Put, "/posts/1", "Test [Updated]", "Body");
        AssertReady(viewModel, factory, 2);

        await viewModel.UpdatePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please select a post to update", viewModel.StatusMessage);
        AssertPost(viewModel.Posts[0], 1, "Updated Post", "Updated body");
        AssertReady(viewModel, factory, 2);
    }

    [TestMethod]
    public async Task DeletePostAsync_WithNullPost_SetsErrorStatus()
    {
        using var response = DeletePostResponse();
        using var factory = new RecordingFactory(() => response);
        var viewModel = new MainWindowViewModel(factory);
        AssertInitialState(viewModel, factory);

        factory.ObservedViewModel = viewModel;
        await viewModel.DeletePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please select a post to delete", viewModel.StatusMessage);
        Assert.AreEqual(0, viewModel.Posts.Count);
        AssertReady(viewModel, factory, 0);

        var post = new Post(1, 1, "Test", "Body");
        viewModel.Posts.Add(post);
        await viewModel.DeletePostCommand.ExecuteAsync(post).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual("✓ Deleted post: Test", viewModel.StatusMessage);
        AssertRequest(factory, 0, HttpMethod.Delete, "/posts/1");
        AssertReady(viewModel, factory, 1);

        await viewModel.DeletePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please select a post to delete", viewModel.StatusMessage);
        Assert.AreEqual(0, viewModel.Posts.Count);
        AssertReady(viewModel, factory, 1);
    }

    [TestMethod]
    public async Task DeletePostAsync_ErrorResponse_SetsErrorStatus()
    {
        using var failedResponse = GetErrorResponse();
        using var recoveredResponse = DeletePostResponse();
        using var factory = new RecordingFactory(() => failedResponse, () => recoveredResponse);
        var viewModel = new MainWindowViewModel(factory);
        var post = new Post(1, 1, "Test", "Body");
        viewModel.Posts.Add(post);

        factory.ObservedViewModel = viewModel;
        await viewModel.DeletePostCommand.ExecuteAsync(post).ConfigureAwait(false);
        Assert.AreEqual(1, viewModel.Posts.Count);
        Assert.AreSame(
            post,
            viewModel.Posts[0],
            "Failed deletion must preserve the displayed post."
        );
        AssertPost(post, 1, "Test", "Body");
        AssertHttpError(viewModel, "deleting");
        AssertRequest(factory, 0, HttpMethod.Delete, "/posts/1");
        AssertReady(viewModel, factory, 1);

        await viewModel.DeletePostCommand.ExecuteAsync(post).ConfigureAwait(false);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual("✓ Deleted post: Test", viewModel.StatusMessage);
        AssertRequest(factory, 1, HttpMethod.Delete, "/posts/1");
        AssertReady(viewModel, factory, 2);

        await viewModel.DeletePostCommand.ExecuteAsync(null).ConfigureAwait(false);
        Assert.AreEqual("✗ Please select a post to delete", viewModel.StatusMessage);
        Assert.AreEqual(0, viewModel.Posts.Count);
        AssertReady(viewModel, factory, 2);
    }

    private static void AssertInitialState(MainWindowViewModel viewModel, RecordingFactory factory)
    {
        Assert.AreEqual("Ready", viewModel.StatusMessage);
        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.AreEqual(string.Empty, viewModel.NewPostTitle);
        Assert.AreEqual(string.Empty, viewModel.NewPostBody);
        Assert.IsNull(viewModel.SelectedPost);
        AssertReady(viewModel, factory, 0);
    }

    private static void AssertReady(
        MainWindowViewModel viewModel,
        RecordingFactory factory,
        int expectedRequests
    )
    {
        Assert.AreEqual(
            expectedRequests,
            factory.Requests.Count,
            "Each valid action must issue exactly one HTTP request; validation must issue none."
        );
        Assert.AreEqual(
            expectedRequests,
            factory.ClientCount,
            "Each HTTP action must own a fresh client."
        );
        Assert.IsFalse(viewModel.IsLoading);
        Assert.IsFalse(viewModel.LoadPostsCommand.IsRunning);
        Assert.IsFalse(viewModel.CreatePostCommand.IsRunning);
        Assert.IsFalse(viewModel.UpdatePostCommand.IsRunning);
        Assert.IsFalse(viewModel.DeletePostCommand.IsRunning);
        Assert.IsTrue(
            viewModel.LoadPostsCommand.CanExecute(null),
            "Loading must be available after completion or failure."
        );
        Assert.IsTrue(
            viewModel.CreatePostCommand.CanExecute(null),
            "Creation must be available after completion or validation."
        );
        Assert.IsTrue(viewModel.UpdatePostCommand.CanExecute(null));
        Assert.IsTrue(viewModel.DeletePostCommand.CanExecute(null));
    }

    private static void AssertPost(Post post, long id, string title, string body)
    {
        Assert.AreEqual(1L, post.UserId);
        Assert.AreEqual(id, post.Id);
        Assert.AreEqual(title, post.Title);
        Assert.AreEqual(body, post.Body);
    }

    private static void AssertLoadedPosts(MainWindowViewModel viewModel)
    {
        Assert.AreEqual(2, viewModel.Posts.Count);
        AssertPost(viewModel.Posts[0], 1, "sunt aut facere", "quia et suscipit");
        AssertPost(viewModel.Posts[1], 2, "qui est esse", "est rerum tempore");
        Assert.AreEqual("✓ Loaded 2 posts", viewModel.StatusMessage);
        Assert.AreEqual(2, viewModel.Posts.Select(post => post.Id).Distinct().Count());
    }

    private static void AssertCreatedPost(MainWindowViewModel viewModel)
    {
        AssertPost(viewModel.Posts[0], 101, "New Post", "Post body");
        Assert.AreEqual(string.Empty, viewModel.NewPostTitle);
        Assert.AreEqual(string.Empty, viewModel.NewPostBody);
        Assert.AreEqual("✓ Created post: New Post", viewModel.StatusMessage);
    }

    private static void AssertHttpError(MainWindowViewModel viewModel, string action)
    {
        Assert.IsTrue(
            viewModel.StatusMessage.StartsWith($"✗ Error {action} post", StringComparison.Ordinal)
        );
        var resource = action == "loading" ? "posts" : "post";
        Assert.AreEqual(
            $"✗ Error {action} {resource}: HTTP BadRequest: {{\"message\":\"Bad Request\"}}",
            viewModel.StatusMessage
        );
    }

    private static void AssertRequest(
        RecordingFactory factory,
        int index,
        HttpMethod method,
        string path,
        string? title = null,
        string? body = null
    )
    {
        var request = factory.Requests[index];
        Assert.AreEqual(method, request.Method);
        Assert.IsTrue(
            request.WasLoading,
            "Loading state must be visible while the HTTP action is executing."
        );
        var expectedInFlightStatus =
            method == HttpMethod.Get ? "Loading posts..."
            : method == HttpMethod.Post ? "Creating post..."
            : method == HttpMethod.Put ? "Updating post..."
            : "Deleting post...";
        Assert.AreEqual(
            expectedInFlightStatus,
            request.InFlightStatus,
            "The status must identify the active action before its response arrives."
        );
        Assert.AreEqual("https://jsonplaceholder.typicode.com" + path, request.Url);
        if (title == null)
        {
            Assert.IsNull(request.Body, "GET and DELETE must not send an unexpected JSON body.");
            return;
        }

        Assert.IsNotNull(request.Body, "POST and PUT must send the user's data.");
        using var json = JsonDocument.Parse(request.Body);
        Assert.AreEqual(
            3,
            json.RootElement.EnumerateObject().Count(),
            "PostInput must send exactly userId, title, and body."
        );
        Assert.AreEqual(1L, json.RootElement.GetProperty("userId").GetInt64());
        Assert.AreEqual(title, json.RootElement.GetProperty("title").GetString());
        Assert.AreEqual(body, json.RootElement.GetProperty("body").GetString());
        Assert.IsFalse(
            json.RootElement.TryGetProperty("id", out _),
            "The server owns new post identifiers."
        );
    }

    private sealed record CapturedRequest(
        HttpMethod Method,
        string Url,
        string? Body,
        bool WasLoading,
        string InFlightStatus
    );

    private sealed class RecordingFactory(params Func<HttpResponseMessage>[] responses)
        : IHttpClientFactory,
            IDisposable
    {
        private readonly Queue<Func<HttpResponseMessage>> responseSteps = new(responses);
        private RecordingHandler? handler;

        public List<CapturedRequest> Requests { get; } = [];

        public int ClientCount { get; private set; }

        public MainWindowViewModel? ObservedViewModel { get; set; }

        public HttpClient CreateClient(string name)
        {
            ClientCount++;
            handler ??= new RecordingHandler(this);
            return new HttpClient(handler, disposeHandler: false);
        }

        public void Dispose() => handler?.Dispose();

        private sealed class RecordingHandler(RecordingFactory factory) : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken
            )
            {
                var body =
                    request.Content == null
                        ? null
                        : await request
                            .Content.ReadAsStringAsync(cancellationToken)
                            .ConfigureAwait(false);
                factory.Requests.Add(
                    new CapturedRequest(
                        request.Method,
                        request.RequestUri?.AbsoluteUri
                            ?? throw new InvalidOperationException("Missing request URI."),
                        body,
                        factory.ObservedViewModel?.IsLoading ?? false,
                        factory.ObservedViewModel?.StatusMessage ?? "Missing observer"
                    )
                );
                Assert.IsTrue(
                    factory.responseSteps.Count > 0,
                    "The UI issued an unexpected HTTP request."
                );
                return factory.responseSteps.Dequeue()();
            }
        }
    }

    #region Helpers
    static HttpResponseMessage GetPostsResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                """
                [
                  {
                    "userId": 1,
                    "id": 1,
                    "title": "sunt aut facere",
                    "body": "quia et suscipit"
                  },
                  {
                    "userId": 1,
                    "id": 2,
                    "title": "qui est esse",
                    "body": "est rerum tempore"
                  }
                ]
                """,
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage CreatePostResponse() =>
        new(HttpStatusCode.Created)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                """
                {
                  "userId": 1,
                  "id": 101,
                  "title": "New Post",
                  "body": "Post body"
                }
                """,
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage UpdatePostResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                """
                {
                  "userId": 1,
                  "id": 1,
                  "title": "Updated Post",
                  "body": "Updated body"
                }
                """,
                Encoding.UTF8,
                "application/json"
            ),
        };

    static HttpResponseMessage DeletePostResponse() =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

    static HttpResponseMessage GetErrorResponse() =>
        new(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                /*lang=json,strict*/
                """{"message":"Bad Request"}""",
                Encoding.UTF8,
                "application/json"
            ),
        };

    #endregion
}
