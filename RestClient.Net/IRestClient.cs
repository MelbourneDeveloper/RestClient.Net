using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public interface IRestClient
    {
        TimeSpan Timeout { get; set; }
        Task DeleteAsync(Uri queryString);
        Task DeleteAsync(Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> GetAsync<TReturn>();
        Task<TReturn> GetAsync<TReturn>(Uri queryString);
        Task<TReturn> GetAsync<TReturn>(Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
    }
}