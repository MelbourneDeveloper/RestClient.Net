using System.Text.Json;

namespace RestClient.Net.CsTest.Utilities;

#pragma warning disable CA1515 // Consider making public types internal
public static class TestDeserializer
#pragma warning restore CA1515 // Consider making public types internal
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task<T> Deserialize<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
        where T : class
    {
        var stream = await response
            .Content.ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        var jsonBody = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        var t =
            typeof(T) == typeof(string)
                ? jsonBody! as T
                : JsonSerializer.Deserialize<T>(jsonBody, JsonOptions)!;
        return t!;
    }
}
