using System;
using Uris;
#pragma warning disable CA2225 // Operator overloads have named alternates

namespace RestClient.Net.Abstractions
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
        AbsoluteUri requestUri
        ) : base(
            headersCollection,
            statusCode,
            httpRequestMethod,
            responseData,
            requestUri) => Body = body;

        public static implicit operator TResponseBody(Response<TResponseBody> response)
            //TODO: This exception could be a bit misleading
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            => response != null && response.Body != null ? response.Body : throw new ArgumentNullException(nameof(response));
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        #endregion
    }
}