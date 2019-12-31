using RestClientDotNet.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public static class CallExtensions
    {
        public static Task<RestResponseBase<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(this IRestClient restClient, RestRequest<TRequestBody> restRequest)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TResponseBody, TRequestBody>(restRequest);
        }

        #region Get
        public static Task<RestResponseBase<TResponseBody>> GetAsync<TResponseBody>(this IRestClient restClient)
        {
            return GetAsync<TResponseBody>(restClient, default(Uri));
        }

        public static Task<RestResponseBase<TResponseBody>> GetAsync<TResponseBody>(this IRestClient restClient, string resource)
        {
            try
            {
                return GetAsync<TResponseBody>(restClient, new Uri(resource, UriKind.Relative));
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

        public static Task<RestResponseBase<TResponseBody>> GetAsync<TResponseBody>(this IRestClient restClient, Uri resource)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<TResponseBody>(restClient, resource, default);
        }

        public static Task<RestResponseBase<TResponseBody>> GetAsync<TResponseBody>(this IRestClient restClient, Uri resource, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, object>(restClient,
                new RestRequest<object>(
                    resource,
                    default,
                    null,
                    HttpRequestMethod.Get,
                    restClient,
                    cancellationToken));
        }
        #endregion

        #region Delete
        public static Task<RestResponseBase> DeleteAsync(this IRestClient restClient, string resource)
        {
            return DeleteAsync(restClient, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase> DeleteAsync(this IRestClient restClient, Uri resource)
        {
            return DeleteAsync(restClient, resource, default);
        }

        public static async Task<RestResponseBase> DeleteAsync(this IRestClient restClient, Uri resource, CancellationToken cancellationToken)
        {
            var response = (RestResponseBase)await SendAsync<object, object>(restClient,
            new RestRequest<object>(
                  resource,
                default,
                null,
                HttpRequestMethod.Delete,
                restClient,
                cancellationToken));

            return response;
        }
        #endregion

        #region Put
        public static async Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, string resource, TRequestBody body)
        {
            return await PutAsync<TResponseBody, TRequestBody>(restClient, new Uri(resource, UriKind.Relative), body);
        }

        public static Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, Uri resource, TRequestBody body)
        {
            return PutAsync<TResponseBody, TRequestBody>(restClient, resource, body, default);
        }

        public static Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, Uri resource, TRequestBody body, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new RestRequest<TRequestBody>(
                    resource,
                    body,
                    null,
                    HttpRequestMethod.Put,
                    restClient,
                    cancellationToken));
        }
        #endregion

        #region Post
        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, default(string), body);
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, string resource, TRequestBody body)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, resource != null ? new Uri(resource, UriKind.Relative) : default, body);
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, Uri resource, TRequestBody body)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, resource, body, default);
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, Uri resource, TRequestBody body, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new RestRequest<TRequestBody>(
                    resource,
                    body,
                    null,
                    HttpRequestMethod.Post,
                    restClient,
                    cancellationToken));
        }
        #endregion

        #region Patch
        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, string resource, TRequestBody body)
        {
            return PatchAsync<TResponseBody, TRequestBody>(restClient, new Uri(resource, UriKind.Relative), body);
        }

        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, Uri resource, TRequestBody body)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TResponseBody, TRequestBody>(restClient, resource, body, default);
        }

        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, Uri resource, TRequestBody body, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new RestRequest<TRequestBody>(
                    resource,
                    body,
                    null,
                    HttpRequestMethod.Patch,
                    restClient,
                    cancellationToken));
        }
        #endregion
    }
}