using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClient
    {
        Task<RestResponse<TReturn>> SendAsync<TReturn, TBody>(Uri queryString, HttpVerb httpVerb, string contentType, TBody body, CancellationToken cancellationToken);
        IRestHeadersCollection DefaultRequestHeaders { get; }
        string DefaultContentType { get; }
        TimeSpan Timeout { get; set; }
        Task DeleteAsync(Uri queryString);
        Task DeleteAsync(Uri queryString, CancellationToken cancellationToken);
        Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
        Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
        Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
    }
}