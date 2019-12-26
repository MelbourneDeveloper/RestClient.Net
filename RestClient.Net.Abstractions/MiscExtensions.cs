using System;
using System.Text;

namespace RestClientDotNet.Abstractions.Extensions
{
    public static class MiscExtensions
    {
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

        public static void UseJsonContentType(this IRestClient restClient)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            if (!restClient.DefaultRequestHeaders.Contains("Content-Type"))
            {
                restClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            }
        }
    }
}