using System;

namespace RestClient.Net.Abstractions
{
    public class HttpStatusException : Exception
    {
        public Response Response { get; }
        public IClient Client { get; }

        public HttpStatusException(string message, Response response, IClient client) : base(message)
        {
            Response = response;
            Client = client;
        }
    }
}