namespace RestClient.Net.CsTest;

public sealed class SimpleFakeHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new();
}
