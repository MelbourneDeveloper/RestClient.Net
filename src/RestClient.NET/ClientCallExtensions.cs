#if !NET45

using RestClient.Net.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net
{
    public static class ClientCallExtensions
    {
        public static async Task<(IClient Client, Response<TResponseBody> Response)> GetAsync<TResponseBody>(
            this AbsoluteUrl url,
            IHeadersCollection? headersCollection = null,
            CancellationToken cancellationToken = default)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            var client = new Client(url.WithRelativeUrl(RelativeUrl.Empty));
            var response = await client.GetAsync<TResponseBody>(
                url.RelativeUrl,
                headersCollection,
                cancellationToken)
                .ConfigureAwait(false) ?? throw new InvalidOperationException("The response was null");
            return (client, response);
        }

        public static async Task<(IClient Client, Response<TResponseBody> Response)> PatchAsync<TResponseBody, TRequestBody>(
            this AbsoluteUrl url,
            TRequestBody? requestBody = default,
            IHeadersCollection? headersCollection = null,
            CancellationToken cancellationToken = default)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            var client = new Client(url.WithRelativeUrl(RelativeUrl.Empty));
            var response = await client.PatchAsync<TResponseBody, TRequestBody>(
                requestBody,
                url.RelativeUrl,
                headersCollection,
                cancellationToken)
                .ConfigureAwait(false) ?? throw new InvalidOperationException("The response was null");
            return (client, response);
        }

        public static async Task<(IClient Client, Response<TResponseBody> Response)> PostAsync<TResponseBody, TRequestBody>(
            this AbsoluteUrl url,
            TRequestBody? requestBody = default,
            IHeadersCollection? headersCollection = null,
            CancellationToken cancellationToken = default)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            var client = new Client(url.WithRelativeUrl(RelativeUrl.Empty));
            var response = await client.PostAsync<TResponseBody, TRequestBody>(
                requestBody,
                url.RelativeUrl,
                headersCollection,
                cancellationToken)
                .ConfigureAwait(false) ?? throw new InvalidOperationException("The response was null");
            return (client, response);
        }

        public static async Task<(IClient Client, Response<TResponseBody> Response)> PutAsync<TResponseBody, TRequestBody>(
            this AbsoluteUrl url,
            TRequestBody? requestBody = default,
            IHeadersCollection? headersCollection = null,
            CancellationToken cancellationToken = default)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            var client = new Client(url.WithRelativeUrl(RelativeUrl.Empty));
            var response = await client.PutAsync<TResponseBody, TRequestBody>(
                requestBody,
                url.RelativeUrl,
                headersCollection,
                cancellationToken)
                .ConfigureAwait(false) ?? throw new InvalidOperationException("The response was null");
            return (client, response);
        }

        public static async Task<(IClient Client, Response Response)> DeleteAsync(
            this AbsoluteUrl url,
            IHeadersCollection? headersCollection = null,
            CancellationToken cancellationToken = default)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            var client = new Client(url.WithRelativeUrl(RelativeUrl.Empty));
            var response = await client.DeleteAsync(
                url.RelativeUrl,
                headersCollection,
                cancellationToken)
                .ConfigureAwait(false) ?? throw new InvalidOperationException("The response was null");
            return (client, response);
        }

    }
}

#endif
