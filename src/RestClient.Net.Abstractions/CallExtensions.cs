using System;
using System.Threading;
using System.Threading.Tasks;
using Urls;

namespace RestClient.Net
{
    public static class CallExtensions
    {
        #region Public Methods

        #region Delete

        public static Task<Response> DeleteAsync(this IClient client, string path)
            => DeleteAsync(client, new RelativeUrl(path));

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
                (resource != null ? client.BaseUrl?.WithRelativeUrl(resource) :
                client.BaseUrl) ?? throw new ArgumentNullException(nameof(resource)),
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
          => client == null ? throw new ArgumentNullException(nameof(client)) :
             GetAsync<TResponseBody>(client, client.BaseUrl.RelativeUrl);

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client, string path)
            => GetAsync<TResponseBody>(client, new RelativeUrl(path));

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(
            this IClient client,
            RelativeUrl? resource = null,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default) => client == null
                ? throw new ArgumentNullException(nameof(client))
                : client.SendAsync<TResponseBody, object>(
                new Request<object>(
                    (resource != null ? client.BaseUrl.WithRelativeUrl(resource) :
                    client.BaseUrl) ?? throw new ArgumentNullException(nameof(resource)),
                    null,
                    client.AppendDefaultRequestHeaders(requestHeaders ?? NullHeadersCollection.Instance),
                    HttpRequestMethod.Get,
                    cancellationToken: cancellationToken));


        //Is this good?

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(
            this IClient client,
            AbsoluteUrl resource,
            IHeadersCollection? requestHeaders = null,
            CancellationToken cancellationToken = default) => client == null
                ? throw new ArgumentNullException(nameof(client))
                : client.SendAsync<TResponseBody, object>(
                new Request<object>(
                    resource,
                    null,
                    client.AppendDefaultRequestHeaders(requestHeaders ?? NullHeadersCollection.Instance),
                    HttpRequestMethod.Get,
                    cancellationToken: cancellationToken));

        #endregion

        #region Patch

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            string path) => PatchAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                new RelativeUrl(path));

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
            TRequestBody? requestBody = default,
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
                requestBody, null, null, default);

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(
            this IClient client,
            TRequestBody requestBody,
            string path)
            => PostAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                new RelativeUrl(path));

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
            string path) => PutAsync<TResponseBody, TRequestBody>(
                client,
                requestBody,
                new RelativeUrl(path));

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
            TRequestBody? requestBody = default,
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
                                (resource != null ? client.BaseUrl?.WithRelativeUrl(resource) :
                                client.BaseUrl) ?? throw new ArgumentNullException(nameof(resource)),
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