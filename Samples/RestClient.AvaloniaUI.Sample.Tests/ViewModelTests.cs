using System.Net;
using System.Text;
using RestClient.AvaloniaUI.Sample.ViewModels;
using static RestClient.Net.CsTest.Fakes.FakeHttpClientFactory;

namespace RestClient.AvaloniaUI.Sample.Tests;

/// <summary>View model tests for MainWindowViewModel using mocked HTTP client.</summary>
[TestClass]
public sealed class MainWindowViewModelTests
{
    [TestMethod]
    public async Task LoadPostsAsync_SuccessResponse_LoadsPostsIntoCollection()
    {
        using var response = GetPostsResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory);

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.AreEqual(2, viewModel.Posts.Count);
        Assert.AreEqual("sunt aut facere", viewModel.Posts[0].Title);
        Assert.IsTrue(viewModel.StatusMessage.Contains("2 posts", StringComparison.Ordinal));
        Assert.IsFalse(viewModel.IsLoading);
    }

    [TestMethod]
    public async Task LoadPostsAsync_ErrorResponse_SetsErrorStatus()
    {
        using var response = GetErrorResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory);

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.IsTrue(
            viewModel.StatusMessage.Contains("Error loading posts", StringComparison.Ordinal)
        );
        Assert.IsFalse(viewModel.IsLoading);
    }

    [TestMethod]
    public async Task LoadPostsAsync_ExceptionThrown_SetsErrorStatus()
    {
        var httpClientFactory = CreateMockHttpClientFactory(
            exceptionToThrow: new HttpRequestException("Network error")
        );
        var viewModel = new MainWindowViewModel(httpClientFactory);

        await viewModel.LoadPostsCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.IsTrue(viewModel.StatusMessage.Contains("Network error", StringComparison.Ordinal));
        Assert.IsFalse(viewModel.IsLoading);
    }

    [TestMethod]
    public async Task CreatePostAsync_WithValidInput_CreatesAndAddsPost()
    {
        using var response = CreatePostResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory)
        {
            NewPostTitle = "New Post",
            NewPostBody = "Post body",
        };

        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.AreEqual(1, viewModel.Posts.Count);
        Assert.AreEqual(101, viewModel.Posts[0].Id);
        Assert.AreEqual("New Post", viewModel.Posts[0].Title);
        Assert.AreEqual(string.Empty, viewModel.NewPostTitle);
        Assert.AreEqual(string.Empty, viewModel.NewPostBody);
        Assert.IsTrue(viewModel.StatusMessage.Contains("Created post", StringComparison.Ordinal));
        Assert.IsFalse(viewModel.IsLoading);
    }

    [TestMethod]
    public async Task CreatePostAsync_WithEmptyTitle_SetsErrorStatus()
    {
        using var response = CreatePostResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory)
        {
            NewPostTitle = string.Empty,
            NewPostBody = "Post body",
        };

        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.IsTrue(
            viewModel.StatusMessage.Contains(
                "Please enter post title and body",
                StringComparison.Ordinal
            )
        );
    }

    [TestMethod]
    public async Task CreatePostAsync_ErrorResponse_SetsErrorStatus()
    {
        using var response = GetErrorResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory)
        {
            NewPostTitle = "New Post",
            NewPostBody = "Post body",
        };

        await viewModel.CreatePostCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.AreEqual(0, viewModel.Posts.Count);
        Assert.IsTrue(
            viewModel.StatusMessage.Contains("Error creating post", StringComparison.Ordinal)
        );
        Assert.IsFalse(viewModel.IsLoading);
    }

    [TestMethod]
    public async Task UpdatePostAsync_WithNullPost_SetsErrorStatus()
    {
        using var response = UpdatePostResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory);

        await viewModel.UpdatePostCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.IsTrue(
            viewModel.StatusMessage.Contains(
                "Please select a post to update",
                StringComparison.Ordinal
            )
        );
    }

    [TestMethod]
    public async Task UpdatePostAsync_ErrorResponse_SetsErrorStatus()
    {
        using var response = GetErrorResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory);
        var testPost = new JSONPlaceholder.Generated.Post
        {
            Id = 1,
            UserId = 1,
            Title = "Test",
            Body = "Body",
        };

        await viewModel.UpdatePostCommand.ExecuteAsync(testPost).ConfigureAwait(false);

        Assert.IsTrue(
            viewModel.StatusMessage.Contains("Error updating post", StringComparison.Ordinal)
        );
        Assert.IsFalse(viewModel.IsLoading);
    }

    [TestMethod]
    public async Task DeletePostAsync_WithNullPost_SetsErrorStatus()
    {
        using var response = DeletePostResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory);

        await viewModel.DeletePostCommand.ExecuteAsync(null).ConfigureAwait(false);

        Assert.IsTrue(
            viewModel.StatusMessage.Contains(
                "Please select a post to delete",
                StringComparison.Ordinal
            )
        );
    }

    [TestMethod]
    public async Task DeletePostAsync_ErrorResponse_SetsErrorStatus()
    {
        using var response = GetErrorResponse();
        var httpClientFactory = CreateMockHttpClientFactory(response: response);
        var viewModel = new MainWindowViewModel(httpClientFactory);
        var testPost = new JSONPlaceholder.Generated.Post
        {
            Id = 1,
            UserId = 1,
            Title = "Test",
            Body = "Body",
        };
        viewModel.Posts.Add(testPost);

        await viewModel.DeletePostCommand.ExecuteAsync(testPost).ConfigureAwait(false);

        Assert.AreEqual(1, viewModel.Posts.Count);
        Assert.IsTrue(
            viewModel.StatusMessage.Contains("Error deleting post", StringComparison.Ordinal)
        );
        Assert.IsFalse(viewModel.IsLoading);
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
