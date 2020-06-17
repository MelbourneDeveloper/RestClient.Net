using System.Net.Http;

namespace RestClient.Net
{
    /// <summary>
    /// Get or create a HttpClient
    /// </summary>
    public delegate HttpClient CreateHttpClient(string name);
}
