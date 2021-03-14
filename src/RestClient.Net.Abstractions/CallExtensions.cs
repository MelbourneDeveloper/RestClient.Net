using RestClient.Net.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public static class CallExtensions
    {
        public static Uri Combine(this Uri? baseUri, Uri? relativeUri)
        =>
        baseUri == null && relativeUri == null ? throw new InvalidOperationException($"{nameof(baseUri)} or {nameof(relativeUri)} must not be null") :
        baseUri == null ? relativeUri is Uri ru && ru.IsAbsoluteUri
            ? ru :
            throw new InvalidOperationException("At least one Uri must be absolute and not null") :
            relativeUri == null ? baseUri.IsAbsoluteUri ? baseUri :
            throw new InvalidOperationException("At least one Uri must be absolute and not null") :
            !baseUri.IsAbsoluteUri ? throw new InvalidOperationException($"{nameof(baseUri)} must be absolute") :
            relativeUri.IsAbsoluteUri ? throw new InvalidOperationException($"{nameof(relativeUri)} must be relative") :
            new Uri(baseUri, relativeUri);


        public static Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(this IClient client, IRequest request) => client == null ? throw new ArgumentNullException(nameof(client)) : client.SendAsync<TResponseBody>(request);

        #region Get
        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client) => GetAsync<TResponseBody>(client, default(Uri));

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client, string? resource)
        {
            try
            {
                return GetAsync<TResponseBody>(client, resource != null ? new Uri(resource, UriKind.Relative) : null);
            }
            catch (UriFormatException ufe)
            {
                if (ufe.Message == "A relative URI cannot be created because the 'uriString' parameter represents an absolute URI.")
                {
                    throw new UriFormatException(Messages.ErrorMessageAbsoluteUriAsString, ufe);
                }

                throw;
            }
        }

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client, Uri? resource = null, IHeadersCollection? requestHeaders = null, CancellationToken cancellationToken = default) => client == null
                ? throw new ArgumentNullException(nameof(client))
                : client.SendAsync<TResponseBody>(
                new Request(
                    client.BaseUri.Combine(resource),
                    null,
                    requestHeaders ?? NullHeadersCollection.Instance,
                    HttpRequestMethod.Get,
                    cancellationToken));
        #endregion

        #region Delete
        public static Task<Response> DeleteAsync(this IClient client, string resource) => DeleteAsync(client, resource != null ? new Uri(resource, UriKind.Relative) : null);

        public static async Task<Response> DeleteAsync(this IClient client, Uri? resource = null, IHeadersCollection? requestHeaders = null, CancellationToken cancellationToken = default)
        {
            //TODO: do we need this? Client is not nullable
            if (client == null) throw new ArgumentNullException(nameof(client));

            var response = (Response)await client.SendAsync<object>(
            new Request(
                resource,
                null,
                requestHeaders ?? NullHeadersCollection.Instance,
                HttpRequestMethod.Delete,
                cancellationToken))
                .ConfigureAwait(false);

            return response;
        }
        #endregion

        #region Put
        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody) => PutAsync<TResponseBody, TRequestBody>(client, requestBody, default);

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, string? resource) => PutAsync<TResponseBody, TRequestBody>(client, requestBody, resource != null ? new Uri(resource, UriKind.Relative) : null);

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody = default, Uri? resource = null, IHeadersCollection? requestHeaders = null, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            requestHeaders ??= NullHeadersCollection.Instance;

            var requestBodyData = requestBody != null ? client.SerializationAdapter.Serialize(requestBody, requestHeaders) : null;

            return SendAsync<TResponseBody, TRequestBody>(client,
                new Request(
                    resource,
                    requestBodyData,
                    requestHeaders,
                    HttpRequestMethod.Put,
                    cancellationToken));
        }
        #endregion

        #region Post
        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody) => PostAsync<TResponseBody, TRequestBody>(client, requestBody, default);

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, string? resource) => PostAsync<TResponseBody, TRequestBody>(client, requestBody, resource != null ? new Uri(resource, UriKind.Relative) : default);

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, Uri? resource, IHeadersCollection? requestHeaders = null, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            requestHeaders ??= NullHeadersCollection.Instance;

            var requestBodyData = client.SerializationAdapter.Serialize(requestBody, requestHeaders);

            return SendAsync<TResponseBody, TRequestBody>(client,
                new Request(
                    resource,
                    requestBodyData,
                    requestHeaders,
                    HttpRequestMethod.Post,
                    cancellationToken));
        }
        #endregion

        #region Patch
        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody) => PatchAsync<TResponseBody, TRequestBody>(client, requestBody, default);

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, string? resource) => PatchAsync<TResponseBody, TRequestBody>(client, requestBody, resource != null ? new Uri(resource, UriKind.Relative) : default);

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, Uri? resource, IHeadersCollection? requestHeaders = null, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            requestHeaders ??= NullHeadersCollection.Instance;

            var requestBodyData = client.SerializationAdapter.Serialize(requestBody, requestHeaders);

            return SendAsync<TResponseBody, TRequestBody>(client,
                new Request(
                    resource,
                    requestBodyData,
                    requestHeaders,
                    HttpRequestMethod.Patch,
                    cancellationToken));
        }
        #endregion
    }
}