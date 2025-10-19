using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSONPlaceholder.Generated;
using RestClient.Net;

#pragma warning disable IDE0010 // Add missing cases

#pragma warning disable CS1591 // Missing XML comment
#pragma warning disable CA1515 // Because an application's API isn't typically referenced from outside the assembly

namespace RestClient.AvaloniaUI.Sample.ViewModels;

public partial class MainWindowViewModel(IHttpClientFactory httpClientFactory) : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Post> _posts = [];

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _newPostTitle = "";

    [ObservableProperty]
    private string _newPostBody = "";

    [ObservableProperty]
    private Post? _selectedPost;

    [RelayCommand]
    private async Task LoadPostsAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading posts...";

        using var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.GetPostsAsync().ConfigureAwait(false);

        switch (result)
        {
            case OkPosts(var posts):
                Posts = new ObservableCollection<Post>(posts);
                StatusMessage = $"✓ Loaded {posts.Count} posts";
                break;
            case ErrorPosts(ExceptionErrorString(var ex)):
                StatusMessage = $"✗ Error loading posts: {ex.Message}";
                break;
            case ErrorPosts(ResponseErrorString(var body, var status, _)):
                StatusMessage = $"✗ Error loading posts: HTTP {status}: {body}";
                break;
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task CreatePostAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPostTitle) || string.IsNullOrWhiteSpace(NewPostBody))
        {
            StatusMessage = "✗ Please enter post title and body";
            return;
        }

        IsLoading = true;
        StatusMessage = "Creating post...";

        var newPost = new PostInput(UserId: 1, Title: NewPostTitle, Body: NewPostBody);

        using var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.CreatePostAsync(newPost, default).ConfigureAwait(false);

        switch (result)
        {
            case OkPost(var post):
                Posts.Insert(0, post);
                StatusMessage = $"✓ Created post: {post.Title}";
                NewPostTitle = "";
                NewPostBody = "";
                break;
            case ErrorPost(ExceptionErrorString(var ex)):
                StatusMessage = $"✗ Error creating post: {ex.Message}";
                break;
            case ErrorPost(ResponseErrorString(var body, var status, _)):
                StatusMessage = $"✗ Error creating post: HTTP {status}: {body}";
                break;
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task UpdatePostAsync(Post? post)
    {
        if (post == null)
        {
            StatusMessage = "✗ Please select a post to update";
            return;
        }

        IsLoading = true;
        StatusMessage = "Updating post...";

        var updatedPost = new PostInput(
            UserId: post.UserId,
            Title: post.Title + " [Updated]",
            Body: post.Body
        );

        using var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient
            .UpdatePostAsync(post.Id, updatedPost, default)
            .ConfigureAwait(false);

        switch (result)
        {
            case OkPost(var returnedPost):
                var index = Posts.IndexOf(post);
                if (index >= 0)
                {
                    Posts[index] = returnedPost;
                }
                StatusMessage = $"✓ Updated post: {returnedPost.Title}";
                break;
            case ErrorPost(ExceptionErrorString(var ex)):
                StatusMessage = $"✗ Error updating post: {ex.Message}";
                break;
            case ErrorPost(ResponseErrorString(var body, var status, _)):
                StatusMessage = $"✗ Error updating post: HTTP {status}: {body}";
                break;
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task DeletePostAsync(Post? post)
    {
        if (post == null)
        {
            StatusMessage = "✗ Please select a post to delete";
            return;
        }

        IsLoading = true;
        StatusMessage = "Deleting post...";

        using var httpClient = httpClientFactory.CreateClient();
        var result = await httpClient.DeletePostAsync(post.Id, default).ConfigureAwait(false);

        switch (result)
        {
            case OkUnit(var _):
                _ = Posts.Remove(post);
                StatusMessage = $"✓ Deleted post: {post.Title}";
                break;
            case ErrorUnit(ExceptionErrorString(var ex)):
                StatusMessage = $"✗ Error deleting post: {ex.Message}";
                break;
            case ErrorUnit(ResponseErrorString(var body, var status, _)):
                StatusMessage = $"✗ Error deleting post: HTTP {status}: {body}";
                break;
        }

        IsLoading = false;
    }
}
