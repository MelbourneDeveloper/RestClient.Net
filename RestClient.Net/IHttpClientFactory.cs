using System;
using System.Net.Http;
using RestClientDotNet.Abstractions;

namespace RestClientDotNet
{
    public interface IHttpClientFactory : IDisposable
    {
        TimeSpan Timeout { get; set; }
        Uri BaseUri { get; }
        IRestHeadersCollection DefaultRequestHeaders { get; }
        HttpClient CreateHttpClient();
    }

}
