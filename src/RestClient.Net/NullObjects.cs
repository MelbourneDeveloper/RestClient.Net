using System.Net.Http;

namespace RestClient.Net
{
    public static class NullObjects
    {
        public static HttpResponseMessage NullHttpResponseMessage { get; } = new();
    }
}
