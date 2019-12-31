using System;

namespace RestClientDotNet.Abstractions
{
    public class HttpStatusException : Exception
    {
        public RestResponseBase RestResponse { get; }
        public IClient RestClient { get; }

        public HttpStatusException(string message, RestResponseBase restResponse, IClient restClient) : base(message)
        {
            RestResponse = restResponse;
            RestClient = restClient;
        }
    }
}