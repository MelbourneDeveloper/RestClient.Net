using System;

namespace RestClientDotNet.Abstractions
{
    public class HttpStatusException : Exception
    {
        public Response RestResponse { get; }
        public IClient RestClient { get; }

        public HttpStatusException(string message, Response restResponse, IClient restClient) : base(message)
        {
            RestResponse = restResponse;
            RestClient = restClient;
        }
    }
}