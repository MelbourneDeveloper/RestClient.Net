using Urls;
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace RestClient.Net
{
    public class Response<TResponseBody> : Response
    {
        #region Public Properties
        public TResponseBody? Body { get; }
        #endregion

        #region Constructors
        public Response(
        IHeadersCollection headersCollection,
        int statusCode,
        HttpRequestMethod httpRequestMethod,
        byte[] responseData,
        TResponseBody? body,
        AbsoluteUrl requestUri
        ) : base(
            headersCollection,
            statusCode,
            httpRequestMethod,
            responseData,
            requestUri) => Body = body;

        public static implicit operator TResponseBody?(Response<TResponseBody> response)
            => response != null && response.Body != null ? response.Body : default;
        #endregion
    }
}