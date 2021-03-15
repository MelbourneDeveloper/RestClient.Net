using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public class Request<TBody> : IRequest<TBody>
    {
        #region Public Properties
#pragma warning disable CA1819 // Properties should not return arrays
        public TBody? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        public IHeadersCollection? Headers { get; }
        public Uri Uri { get; }
        public HttpRequestMethod HttpRequestMethod { get; }
        public CancellationToken CancellationToken { get; }
        public string? CustomHttpRequestMethod { get; }
        #endregion

        /// <summary>
        /// Construct a Request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="bodyData"></param>
        /// <param name="headers"></param>
        /// <param name="httpRequestMethod"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="customHttpRequestMethod"></param>
        /// 
        public Request(
            Uri uri,
            TBody? bodyData,
            IHeadersCollection headers,
            HttpRequestMethod httpRequestMethod,
            CancellationToken cancellationToken,
            string? customHttpRequestMethod = null)
        {
            BodyData = bodyData;
            Uri = uri;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;
            CustomHttpRequestMethod = customHttpRequestMethod;
            Headers = headers;

            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (!uri.IsAbsoluteUri) throw new InvalidOperationException($"{nameof(uri)} must be an absolute Uri. Try using one of the extension methods to build the request");
        }

        public override string ToString() => $"\r\nResource: {Uri}\r\nHeaders: {Headers} Method: {HttpRequestMethod}";


    }
}