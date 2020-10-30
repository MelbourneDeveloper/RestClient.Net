using System;

namespace RestClient.Net.Abstractions
{
    public class HttpStatusException : Exception
    {
        public IResponse Response { get; }
        public IClient Client { get; }

        public HttpStatusException(
            string message,
            IResponse response,
            IClient client) : base(message)
        {
            Response = response;
            Client = client;
        }
    }
}