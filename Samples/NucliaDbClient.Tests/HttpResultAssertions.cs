using NucliaDB.Generated;
using Outcome;
using Xunit;

namespace NucliaDbClient.Tests;

internal static class HttpResultAssertions
{
    internal static void Success<TValue, TError>(
        Result<TValue, HttpError<TError>> result,
        TValue expected
    )
    {
        Assert.True(result.IsOk);
        Assert.False(result.IsError);
        var success = Assert.IsType<Result<TValue, HttpError<TError>>.Ok<
            TValue,
            HttpError<TError>
        >>(result);
        Assert.Equal(expected, success.Value);
        var successCalls = 0;
        var errorCalls = 0;
        var tapped = result.Tap(
            value =>
            {
                successCalls++;
                Assert.Equal(expected, value);
            },
            _ =>
            {
                errorCalls++;
            }
        );
        Assert.Same(result, tapped);
        Assert.Equal(1, successCalls);
        Assert.Equal(0, errorCalls);
        var matched = result.Match(
            value =>
            {
                successCalls++;
                Assert.Equal(expected, value);
                return "success";
            },
            _ =>
            {
                errorCalls++;
                return "error";
            }
        );
        Assert.Equal("success", matched);
        Assert.Equal(2, successCalls);
        Assert.Equal(0, errorCalls);
        var bound = result.Bind(value =>
        {
            successCalls++;
            Assert.Equal(expected, value);
            return result;
        });
        Assert.Same(result, bound);
        Assert.Equal(3, successCalls);
        Assert.Equal(0, errorCalls);
        Assert.Equal(expected, result.GetValueOrDefault(default(TValue)!));
    }

    internal static void KnowledgeBox(
        KnowledgeBoxObj kb,
        string expectedId,
        string expectedSlug,
        string expectedTitle
    )
    {
        Assert.Equal(expectedId, kb.Uuid);
        Assert.False(string.IsNullOrWhiteSpace(kb.Uuid));
        Assert.Equal(expectedSlug, kb.Slug);
        Assert.NotNull(kb.Config);
        Assert.Equal(expectedSlug, kb.Config.Slug);
        Assert.Equal(expectedTitle, kb.Config.Title);
        Assert.False(kb.Config.HiddenResourcesEnabled);
        Assert.False(kb.Config.HiddenResourcesHideOnCreation);
        var json = System.Text.Json.JsonSerializer.Serialize(kb);
        var restored = System.Text.Json.JsonSerializer.Deserialize<KnowledgeBoxObj>(json);
        Assert.NotNull(restored);
        Assert.NotSame(kb, restored);
        Assert.Equal(kb.Uuid, restored.Uuid);
        Assert.Equal(kb.Slug, restored.Slug);
        Assert.NotNull(restored.Config);
        Assert.Equal(kb.Config.Slug, restored.Config.Slug);
        Assert.Equal(kb.Config.Title, restored.Config.Title);
        Assert.Equal(kb.Config.HiddenResourcesEnabled, restored.Config.HiddenResourcesEnabled);
        Assert.Equal(
            kb.Config.HiddenResourcesHideOnCreation,
            restored.Config.HiddenResourcesHideOnCreation
        );
        Assert.Equal(kb.Model, restored.Model);
    }
}
