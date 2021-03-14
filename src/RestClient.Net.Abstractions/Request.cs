using System;
using System.Threading;

namespace RestClient.Net.Abstractions
{
    public class Request : IRequest
    {
        #region Public Properties
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[]? BodyData { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        public IHeadersCollection? Headers { get; }
        public Uri? Resource { get; }
        public HttpRequestMethod HttpRequestMethod { get; }
        public CancellationToken CancellationToken { get; }
        public string? CustomHttpRequestMethod { get; }
        #endregion

        /// <summary>
        /// Construct a Request
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="bodyData"></param>
        /// <param name="headers"></param>
        /// <param name="httpRequestMethod"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="customHttpRequestMethod"></param>
        /// 
        public Request(
            Uri? resource,
            byte[]? bodyData,
            IHeadersCollection headers,
            HttpRequestMethod httpRequestMethod,
            CancellationToken cancellationToken,
            string? customHttpRequestMethod = null)
        {
            BodyData = bodyData;
            Resource = resource;
            HttpRequestMethod = httpRequestMethod;
            CancellationToken = cancellationToken;
            CustomHttpRequestMethod = customHttpRequestMethod;

            Headers = headers;
        }

        public override string ToString() => $"\r\nResource: {Resource}\r\nHeaders: {Headers} Method: {HttpRequestMethod}";


    }
}