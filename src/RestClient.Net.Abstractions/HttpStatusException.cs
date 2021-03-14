using System;

namespace RestClient.Net.Abstractions
{
    public class HttpStatusException : Exception
    {
        public IResponse Response { get; }

        public HttpStatusException(
            string message,
            IResponse response) : base(message) => Response = response;
    }
}