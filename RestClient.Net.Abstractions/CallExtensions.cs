using RestClient.Net.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public static class CallExtensions
    {
        public static Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(this IClient client, Request<TRequestBody> request)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            return client.SendAsync<TResponseBody, TRequestBody>(request);
        }

        #region Get
        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client)
        {
            return GetAsync<TResponseBody>(client, default(Uri));
        }

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client, string resource)
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

        public static Task<Response<TResponseBody>> GetAsync<TResponseBody>(this IClient client, Uri resource = null, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, object>(client,
                new Request<object>(
                    resource,
                    default,
                    requestHeaders,
                    HttpRequestMethod.Get,
                    client,
                    cancellationToken));
        }
        #endregion

        #region Delete
        public static Task<Response> DeleteAsync(this IClient client, string resource)
        {
            return DeleteAsync(client, resource != null ? new Uri(resource, UriKind.Relative) : null);
        }

        public static async Task<Response> DeleteAsync(this IClient client, Uri resource = null, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            var response = (Response)await SendAsync<object, object>(client,
            new Request<object>(
                  resource,
                default,
                requestHeaders,
                HttpRequestMethod.Delete,
                client,
                cancellationToken));

            return response;
        }
        #endregion

        #region Put
        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody)
        {
            return PutAsync<TResponseBody, TRequestBody>(client, requestBody, default);
        }

        public static async Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, string resource)
        {
            return await PutAsync<TResponseBody, TRequestBody>(client, requestBody, resource != null ? new Uri(resource, UriKind.Relative) : null);
        }

        public static Task<Response<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody = default, Uri resource = null, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, TRequestBody>(client,
                new Request<TRequestBody>(
                    resource,
                    requestBody,
                    headers: requestHeaders,
                    HttpRequestMethod.Put,
                    client,
                    cancellationToken));
        }
        #endregion

        #region Post
        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody)
        {
            return PostAsync<TResponseBody, TRequestBody>(client, requestBody, default);
        }

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, string resource)
        {
            return PostAsync<TResponseBody, TRequestBody>(client, requestBody, resource != null ? new Uri(resource, UriKind.Relative) : default);
        }

        public static Task<Response<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, Uri resource, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, TRequestBody>(client,
                new Request<TRequestBody>(
                    resource,
                    requestBody,
                    requestHeaders,
                    HttpRequestMethod.Post,
                    client,
                    cancellationToken));
        }
        #endregion

        #region Patch
        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody)
        {
            return PatchAsync<TResponseBody, TRequestBody>(client, requestBody, default);
        }

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, string resource)
        {
            return PatchAsync<TResponseBody, TRequestBody>(client, requestBody, resource != null ? new Uri(resource, UriKind.Relative) : default);
        }

        public static Task<Response<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IClient client, TRequestBody requestBody, Uri resource, IHeadersCollection requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return SendAsync<TResponseBody, TRequestBody>(client,
                new Request<TRequestBody>(
                    resource,
                    requestBody,
                    requestHeaders,
                    HttpRequestMethod.Patch,
                    client,
                    cancellationToken));
        }
        #endregion
    }
}