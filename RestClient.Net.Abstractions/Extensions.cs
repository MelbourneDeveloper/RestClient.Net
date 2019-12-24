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

        public static Task<TBody> DeserializeResponseBodyAsync<TBody>(this IRestClient restClient, RestResponseBase response)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            if (response == null) throw new ArgumentNullException(nameof(response));
            return restClient.SerializationAdapter.DeserializeAsync<TBody>(response.GetResponseData());
        }
        #endregion

        #region Get
        public static Task<RestResponseBase<T>> GetAsync<T>(this IRestClient restClient)
        {
            return GetAsync<T>(restClient, default(Uri));
        }

        public static Task<RestResponseBase<T>> GetAsync<T>(this IRestClient restClient, string resource)
        {
            try
            {
                return GetAsync<T>(restClient, new Uri(resource, UriKind.Relative));
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

        public static Task<RestResponseBase<T>> GetAsync<T>(this IRestClient restClient, Uri resource)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<T>(restClient, resource, restClient.DefaultContentType);
        }

        public static Task<RestResponseBase<T>> GetAsync<T>(this IRestClient restClient, Uri resource, string contentType)
        {
            return GetAsync<T>(restClient, resource, contentType, default);
        }

        public static Task<RestResponseBase<T>> GetAsync<T>(this IRestClient restClient, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<T>(restClient, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<T>> GetAsync<T>(this IRestClient restClient, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<T, object>(
                new RestRequest<object>(
                    default,
                    null,
                    restClient,
                    resource,
                    HttpVerb.Get,
                    contentType,
                    cancellationToken)
                );
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
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return await restClient.SendAsync<object, object>(resource, HttpVerb.Delete, restClient.DefaultContentType, null, cancellationToken);
        }
        #endregion

        #region Put
        public static async Task<RestResponseBase<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string resource)
        {
            return await PutAsync<TReturn, TBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource)
        {
            return PutAsync<TReturn, TBody>(restClient, body, resource, default);
        }

        public static Task<RestResponseBase<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PutAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, TBody>(resource, HttpVerb.Put, contentType, body, cancellationToken);
        }
        #endregion

        #region Post
        public static Task<RestResponseBase<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body)
        {
            return PostAsync<TReturn, TBody>(restClient, body, default(Uri));
        }

        public static Task<RestResponseBase<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string resource)
        {
            return PostAsync<TReturn, TBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource)
        {
            return PostAsync<TReturn, TBody>(restClient, body, resource, default);
        }

        public static Task<RestResponseBase<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PostAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, TBody>(resource, HttpVerb.Post, contentType, body, cancellationToken);
        }
        #endregion

        #region Patch
        public static Task<RestResponseBase<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string resource)
        {
            return PatchAsync<TReturn, TBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponseBase<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, default);
        }

        public static Task<RestResponseBase<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponseBase<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, object>(resource, HttpVerb.Patch, contentType, body, cancellationToken);
        }
        #endregion
    }
}
