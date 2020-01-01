using System;

namespace RestClient.Net.Abstractions
{
    public class HttpStatusException : Exception
    {
        public Response Response { get; }
        public IClient RestClient { get; }

        public HttpStatusException(string message, Response response, IClient restClient) : base(message)
        {
            Response = response;
            RestClient = restClient;
        }
    }
}