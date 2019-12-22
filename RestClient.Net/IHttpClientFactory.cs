using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public interface IHttpClientFactory
    {
        TimeSpan Timeout { get; set; }
        Uri BaseUri { get; }
        HttpClient Create();
    }

}
