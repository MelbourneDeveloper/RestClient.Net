using System;

namespace RestClientDotNet.Abstractions
{
    public class HttpStatusException : Exception
    {
        public RestResponseBase RestResponse { get; }
        public IRestClient RestClient { get; }

        public HttpStatusException(string message, RestResponseBase restResponse, IRestClient restClient) : base(message)
        {
            RestResponse = restResponse;
            RestClient = restClient;
        }
    }
}