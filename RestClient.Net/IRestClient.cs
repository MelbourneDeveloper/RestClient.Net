using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public interface IRestClient
    {
        Task DeleteAsync(Uri queryString);
        Task DeleteAsync(Uri queryString, CancellationToken cancellationToken);
        Task<T> GetAsync<T>();
        Task<T> GetAsync<T>(Uri queryString);
        Task<T> GetAsync<T>(Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<TReturn> PatchAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<TReturn> PostAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
        Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString);
        Task<TReturn> PutAsync<TReturn, TBody>(TBody body, Uri queryString, CancellationToken cancellationToken);
    }
}