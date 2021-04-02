using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net
{
    public static class CallExtensions
    {
        #region Fields
        //private const string MessageAtLeastOneUri = "At least one Uri must be absolute and not null";
        #endregion

        #region Private Methods
        /// <summary>
        /// Uris don't concatenate properly if the first Uri doesn't end with a forward slash. this is a known bug in .NET
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns></returns>
        //private static Uri AddForwardSlashIfNecessary(this Uri baseUri)
        //    => baseUri == null ? throw new ArgumentNullException(nameof(baseUri)) :
        //    !baseUri.ToString().EndsWith("/", StringComparison.OrdinalIgnoreCase) ? new Uri($"{baseUri}/") : baseUri;

        #endregion

        #region Public Methods

        #region Misc

        /// <summary>
        /// Combines two Uris in a safe way. 
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="RelativeUrl"></param>
        /// <returns></returns>
        //public static Uri Combine(this Uri? baseUri, Uri? RelativeUrl)
        //=>
        //baseUri == null && RelativeUrl == null ? throw new InvalidOperationException($"{nameof(baseUri)} or {nameof(RelativeUrl)} must not be null") :
        //baseUri == null ? RelativeUrl is Uri ru && ru.IsAbsoluteUrl
        //    ? ru :
        //    throw new InvalidOperationException(MessageAtLeastOneUri) :
        //    RelativeUrl == null ? baseUri.IsAbsoluteUrl ? baseUri :
        //    throw new InvalidOperationException(MessageAtLeastOneUri) :
        //    !baseUri.IsAbsoluteUrl ? throw new InvalidOperationException($"{nameof(baseUri)} must be absolute") :
        //    RelativeUrl.IsAbsoluteUrl ? throw new InvalidOperationException($"{nameof(RelativeUrl)} must be relative") :
        //    new Uri(baseUri.AddForwardSlashIfNecessary(), RelativeUrl);
        #endregion

        #region Delete

        public static Task<Response> DeleteAsync(this IClient client, string resource)
            => DeleteAsync(client, resource != null ? new Uri(resource, UriKind.Relative).ToAbsoluteUrl().RelativeUrl : null);

        public static async Task<Response> DeleteAsync(
            this IClient client,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
        {
            //TODO: do we need this? Client is not nullable
            if (client == null) throw new ArgumentNullException(nameof(client));

            var response = (Response)await client.SendAsync<object, object>(
            new Request<object>(
                (resource != null ? client.BaseUri?.WithRelativeUrl(resource) :
                client.BaseUri) ?? throw new ArgumentNullException(nameof(resource)),
                null,
                client.AppendDefaultRequestHeaders(requestHeaders ?? NullHeadersCollection.Instance),
                HttpRequestMethod.Delete,
                cancellationToken: cancellationToken))
                .ConfigureAwait(false);

            return response;
        }

        #endregion

        #region Get

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client)
            => GetAsync<TResponseBody>(client, default(RelativeUrl));

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client, string? resource)
            => GetAsync<TResponseBody>(client, resource != null ? new Uri(resource, UriKind.Relative).ToAbsoluteUrl().RelativeUrl : null);

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(
            this IClient client,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default) => client == null
                ? throw new ArgumentNullException(nameof(client))
                : client.SendAsync<TResponseBody, object>(
                new Request<object>(
                    (resource != null ? client.BaseUri?.WithRelativeUrl(resource) :
                    client.BaseUri) ?? throw new ArgumentNullException(nameof(resource)),
                    null,
                    client.AppendDefaultRequestHeaders(requestHeaders ?? NullHeadersCollection.Instance),
                    HttpRequestMethod.Get,
                    cancellationToken: cancellationToken));

        #endregion

        #region Patch

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            string resource) => PatchAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                resource != null ? new Uri(resource, UriKind.Relative).ToAbsoluteUrl().RelativeUrl : default);

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody>(
        this IClient client,
        RelativeUrl? resource = null,
        IHeadersCollection? requestHeaders = null,
        CancellationToken cancellationToken = default)
            => SendAsync<TResponseBody, object>(
                client,
                HttpRequestMethod.Patch,
                default,
                resource,
                requestHeaders,
                cancellationToken);


        public static async Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(
        this IClient client,
        TRequestBody request,
        TimeSpan timeout,
        RelativeUrl? resource = null,
        IHeadersCollection? requestHeaders = null)
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeout);

            return await SendAsync<TResponseBody, object>(
                client,
                HttpRequestMethod.Patch,
                request,
                resource,
                requestHeaders,
                cancellationTokenSource.Token).ConfigureAwait(false);
        }

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
            => SendAsync<TResponseBody, TRequestBody>(
                client,
                HttpRequestMethod.Patch,
                requestBody,
                resource,
                requestHeaders,
                cancellationToken);

        #endregion

        #region Post

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody)
            => PostAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                default);

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            string? resource)
            => PostAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                resource != null ? new Uri(resource, UriKind.Relative).ToAbsoluteUrl().RelativeUrl : default);

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody>(
            this IClient client,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
            => SendAsync<TResponseBody, object>(
                client,
                HttpRequestMethod.Post,
                default,
                resource,
                requestHeaders,
                cancellationToken);

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody? requestBody = default,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
            => SendAsync<TResponseBody, TRequestBody>(
                client,
                HttpRequestMethod.Post,
                requestBody,
                resource,
                requestHeaders,
                cancellationToken);

        #endregion

        #region Put

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            string? resource) => PutAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                resource != null ? new Uri(resource, UriKind.Relative).ToAbsoluteUrl().RelativeUrl : null);

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody>(
            this IClient client,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
            => PutAsync<TResponseBody, object>(
                client,
                HttpRequestMethod.Put,
                resource,
                requestHeaders,
                cancellationToken);

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
            => SendAsync<TResponseBody, TRequestBody>(
                client,
                HttpRequestMethod.Put,
                requestBody,
                resource,
                requestHeaders,
                cancellationToken);

        #endregion

        #region Send
        public static Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(
            IClient client,
            HttpRequestMethod httpRequestMethod,
            TRequestBody? requestBody,
            RelativeUrl? resource,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            requestHeaders = client.AppendDefaultRequestHeaders(requestHeaders ?? NullHeadersCollection.Instance);

            return SendAsync<TResponseBody, TRequestBody>(
                client,
                resource,
                requestBody,
                requestHeaders,
                httpRequestMethod,
                cancellationToken);
        }

        public static Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(
            IClient client,
            RelativeUrl? resource,
            TRequestBody? requestBodyData,
            IHeadersCollection requestHeaders,
            HttpRequestMethod httpRequestMethod,
            CancellationToken cancellationToken)
            =>
             client != null ? SendAsync<TResponseBody, TRequestBody>(client,
                            new Request<TRequestBody>(
                                (resource != null ? client.BaseUri?.WithRelativeUrl(resource) :
                                client.BaseUri) ?? throw new ArgumentNullException(nameof(resource)),
                                requestBodyData,
                                requestHeaders,
                                httpRequestMethod,
                                cancellationToken: cancellationToken)) : throw new ArgumentNullException(nameof(client));


        public static Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(
            this IClient client,
            IRequest<TRequestBody> request)
            =>
            client == null ? throw new ArgumentNullException(nameof(client)) : request != null ? client.SendAsync<TResponseBody, TRequestBody>(request) : throw new ArgumentNullException(nameof(request));

        #endregion

        #endregion Public Methods
    }
}