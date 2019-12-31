using RestClientDotNet.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public static class CallExtensions
    {
        public static Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(this IClient restClient, Request<TRequestBody> restRequest)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TResponseBody, TRequestBody>(restRequest);
        }

        #region Get
        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient restClient)
        {
            return GetAsync<TResponseBody>(restClient, default(Uri));
        }

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient restClient, string resource)
        {
            try
            {
                return GetAsync<TResponseBody>(restClient, resource != null ? new Uri(resource, UriKind.Relative) : null);
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

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient restClient, Uri resource = null, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, object>(restClient,
                new Request<object>(
                    resource,
                    default,
                    requestHeaders,
                    HttpRequestMethod.Get,
                    restClient,
                    cancellationToken));
        }
        #endregion

        #region Delete
        public static Task<Response> DeleteAsync(this IClient restClient, string resource)
        {
            return DeleteAsync(restClient, resource != null ? new Uri(resource, UriKind.Relative) : null);
        }

        public static async Task<Response> DeleteAsync(this IClient restClient, Uri resource = null, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            var response = (Response)await SendAsync<object, object>(restClient,
            new Request<object>(
                  resource,
                default,
                requestHeaders,
                HttpRequestMethod.Delete,
                restClient,
                cancellationToken));

            return response;
        }
        #endregion

        #region Put
        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body)
        {
            return PutAsync<TResponseBody, TRequestBody>(restClient, body, default);
        }

        public static async Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body, string resource)
        {
            return await PutAsync<TResponseBody, TRequestBody>(restClient, body, resource != null ? new Uri(resource, UriKind.Relative) : null);
        }

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body = default, Uri resource = null, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new Request<TRequestBody>(
                    resource,
                    body,
                    headers: requestHeaders,
                    HttpRequestMethod.Put,
                    restClient,
                    cancellationToken));
        }
        #endregion

        #region Post
        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, body, default);
        }

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body, string resource)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, body, resource != null ? new Uri(resource, UriKind.Relative) : default);
        }

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body, Uri resource, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new Request<TRequestBody>(
                    resource,
                    body,
                    requestHeaders,
                    HttpRequestMethod.Post,
                    restClient,
                    cancellationToken));
        }
        #endregion

        #region Patch
        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body)
        {
            return PatchAsync<TResponseBody, TRequestBody>(restClient, body, default);
        }

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body, string resource)
        {
            return PatchAsync<TResponseBody, TRequestBody>(restClient, body, resource != null ? new Uri(resource, UriKind.Relative) : default);
        }

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient restClient, TRequestBody body, Uri resource, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new Request<TRequestBody>(
                    resource,
                    body,
                    requestHeaders,
                    HttpRequestMethod.Patch,
                    restClient,
                    cancellationToken));
        }
        #endregion
    }
}