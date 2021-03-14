using System.Net.Http;

namespace RestClient.Net
{
    public static class NullObjects
    {
        public static HttpClient NullHttpClient { get; } = new();
        public static HttpResponseMessage NullHttpResponseMessage { get; } = new();

    }
}
