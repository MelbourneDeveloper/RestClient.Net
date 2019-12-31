using System;
using System.Text;

namespace RestClient.Net.Abstractions.Extensions
{
    public static class MiscExtensions
    {
        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static void SetBasicAuthenticationHeader(this IClient restClient, string userName, string password)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            restClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
        }

        public static void SetBearerTokenuthenticationHeader(this IClient restClient, string bearerToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            restClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
        }

        public static TResponseBody DeserializeResponseBody<TResponseBody>(this IClient restClient, Response response)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            if (response == null) throw new ArgumentNullException(nameof(response));
            return restClient.SerializationAdapter.Deserialize<TResponseBody>(response.GetResponseData(), response.Headers);
        }

        public static void SetJsonContentTypeHeader(this IClient restClient)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            if (!restClient.DefaultRequestHeaders.Contains("Content-Type"))
            {
                restClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            }
            else
            {
                throw new ValidationException(Messages.ErrorMessageHeaderAlreadyExists);
            }
        }
    }
}