using System;
using System.Text;

namespace RestClient.Net.Abstractions.Extensions
{
    public static class MiscExtensions
    {
        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static IHeadersCollection SetBasicAuthenticationHeader(this IHeadersCollection requestHeaders, string userName, string password)
        {
            if (requestHeaders == null) throw new ArgumentNullException(nameof(requestHeaders));
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            return requestHeaders.Append("Authorization", "Basic " + credentials);
        }

        public static IHeadersCollection SetBearerTokenAuthenticationHeader(this IHeadersCollection requestHeaders, string bearerToken)
        {
            return requestHeaders == null
                ? throw new ArgumentNullException(nameof(requestHeaders))
                : requestHeaders.Append("Authorization", "Bearer " + bearerToken);
        }

        public static TResponseBody DeserializeResponseBody<TResponseBody>(this IClient restClient, byte[] response, IHeadersCollection headersCollection) => restClient == null
                ? throw new ArgumentNullException(nameof(restClient))
                : response == null
                ? throw new ArgumentNullException(nameof(response))
                : restClient.SerializationAdapter.Deserialize<TResponseBody>(response, headersCollection);

        public static IHeadersCollection SetJsonContentTypeHeader(this IHeadersCollection requestHeaders)
            => requestHeaders == null
                ? throw new ArgumentNullException(nameof(requestHeaders))
                : !requestHeaders.Contains(ContentTypeHeaderName)
                ? requestHeaders.Append(ContentTypeHeaderName, JsonMediaType)
                : throw new ValidationException(Messages.ErrorMessageHeaderAlreadyExists);


        public const string ContentTypeHeaderName = "Content-Type";
        public const string JsonMediaType = "application/json";

    }
}