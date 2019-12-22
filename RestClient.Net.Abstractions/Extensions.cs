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
        #endregion

        #region Get
        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient)
        {
            return GetAsync<T>(restClient, default(Uri));
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, string queryString)
        {
            try
            {
                return GetAsync<T>(restClient, new Uri(queryString, UriKind.Relative));
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

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri queryString)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<T>(restClient, queryString, restClient.DefaultContentType);
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri queryString, string contentType)
        {
            return GetAsync<T>(restClient, queryString, contentType, default);
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri queryString, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<T>(restClient, queryString, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<T, object>(queryString, HttpVerb.Get, contentType, null, cancellationToken);
        }
        #endregion

        #region Delete
        public static Task DeleteAsync(this IRestClient restClient, string queryString)
        {
            return DeleteAsync(restClient, new Uri(queryString, UriKind.Relative));
        }

        public static Task DeleteAsync(this IRestClient restClient, Uri queryString)
        {
            return DeleteAsync(restClient, queryString, default);
        }

        public static Task DeleteAsync(this IRestClient restClient, Uri queryString, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return DeleteAsync(restClient, queryString, restClient.DefaultContentType, cancellationToken);
        }

        public static Task DeleteAsync(this IRestClient restClient, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<object, object>(queryString, HttpVerb.Delete, contentType, null, cancellationToken);
        }
        #endregion

        #region Put
        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string queryString)
        {
            return PutAsync<TReturn, TBody>(restClient, body, new Uri(queryString, UriKind.Relative));
        }

        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString)
        {
            return PutAsync<TReturn, TBody>(restClient, body, queryString, default);
        }

        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PutAsync<TReturn, TBody>(restClient, body, queryString, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, TBody>(queryString, HttpVerb.Put, contentType, body, cancellationToken);
        }
        #endregion

        #region Post
        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body)
        {
            return PostAsync<TReturn, TBody>(restClient, body, default(Uri));
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string queryString)
        {
            return PostAsync<TReturn, TBody>(restClient, body, new Uri(queryString, UriKind.Relative));
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString)
        {
            return PostAsync<TReturn, TBody>(restClient, body, queryString, default);
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PostAsync<TReturn, TBody>(restClient, body, queryString, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, TBody>(queryString, HttpVerb.Post, contentType, body, cancellationToken);
        }
        #endregion

        #region Patch
        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string queryString)
        {
            return PatchAsync<TReturn, TBody>(restClient, body, new Uri(queryString, UriKind.Relative));
        }

        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TReturn, TBody>(restClient, body, queryString, restClient.DefaultContentType, default);
        }

        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TReturn, TBody>(restClient, body, queryString, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri queryString, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, object>(queryString, HttpVerb.Patch, contentType, body, cancellationToken);
        }
        #endregion
    }
}
