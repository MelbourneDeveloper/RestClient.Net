using System;
using System.Threading;
using Urls;

namespace RestClient.Net
{
    public class Request<TBody> : IRequest<TBody>
    {
        #region Public Properties
#pragma warning disable CA1819 // Properties should not return arrays
        public TBody? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        public IHeadersCollection Headers { get; }
        public AbsoluteUrl Uri { get; }
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
        /// <param name="customHttpRequestMethod"></param>
        /// <param name="cancellationToken"></param>
        /// 
        public Request(
            AbsoluteUrl uri,
            TBody? bodyData,
            IHeadersCollection headers,
            HttpRequestMethod httpRequestMethod,
            string? customHttpRequestMethod = null,
            CancellationToken cancellationToken = default)
        {
            BodyData = bodyData;
            Uri = uri;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;
            CustomHttpRequestMethod = customHttpRequestMethod;
            Headers = headers;

            if (uri == null) throw new ArgumentNullException(nameof(uri));
        }

        public override string ToString() => $"\r\nResource: {Uri}\r\nHeaders: {Headers} Method: {HttpRequestMethod}";


    }
}