using RestClientDotNet.Abstractions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public static class Extensions
    {
        #region Misc
        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static void UseBasicAuthentication(this IRestClient restClient, string userName, string password)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            restClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
        }

        public static TResponseBody DeserializeResponseBodyAsync<TResponseBody>(this IRestClient restClient, RestResponseBase response)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            if (response == null) throw new ArgumentNullException(nameof(response));
            return restClient.SerializationAdapter.Deserialize<TResponseBody>(response.GetResponseData(), response.Headers);
        }

        public static Task<RestResponseBase<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(this IRestClient restClient, RestRequest<TRequestBody> restRequest)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TResponseBody, TRequestBody>(restRequest);
        }
        #endregion

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
                    HttpVerb.Get,
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
                HttpVerb.Delete,
                restClient,
                null,
                cancellationToken));

            return response;
        }
        #endregion

        #region Put
        public static async Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, string resource)
        {
            return await PutAsync<TResponseBody, TRequestBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource)
        {
            return PutAsync<TResponseBody, TRequestBody>(restClient, body, resource, default);
        }

        public static Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PutAsync<TResponseBody, TRequestBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<TResponseBody>> PutAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new RestRequest<TRequestBody>(
                    resource,
                    body,
                    null,
                    HttpVerb.Put,
                    restClient,
                    contentType,
                    cancellationToken));
        }
        #endregion

        #region Post
        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, body, default(Uri));
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, string resource)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource)
        {
            return PostAsync<TResponseBody, TRequestBody>(restClient, body, resource, default);
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PostAsync<TResponseBody, TRequestBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<TResponseBody>> PostAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new RestRequest<TRequestBody>(
                    resource,
                    body,
                    null,
                    HttpVerb.Post,
                    restClient,
                    contentType,
                    cancellationToken));
        }
        #endregion

        #region Patch
        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, string resource)
        {
            return PatchAsync<TResponseBody, TRequestBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TResponseBody, TRequestBody>(restClient, body, resource, restClient.DefaultContentType, default);
        }

        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TResponseBody, TRequestBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<TResponseBody>> PatchAsync<TResponseBody, TRequestBody>(this IRestClient restClient, TRequestBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            return SendAsync<TResponseBody, TRequestBody>(restClient,
                new RestRequest<TRequestBody>(
                    resource,
                    body,
                    null,
                    HttpVerb.Patch,
                    restClient,
                    contentType,
                    cancellationToken));
        }
        #endregion
    }
}
