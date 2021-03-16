using System;
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
        Uri? requestUri
        ) : base(
            headersCollection,
            statusCode,
            httpRequestMethod,
            responseData,
            requestUri) => Body = body;

        public static implicit operator TResponseBody(Response<TResponseBody> readResult)
            //TODO: This exception could be a bit misleading
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            => readResult != null && readResult.Body != null ? readResult.Body : throw new ArgumentNullException(nameof(readResult));
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        #endregion
    }
}