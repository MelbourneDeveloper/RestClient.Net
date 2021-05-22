using System;

namespace RestClient.Net
{
    public class HttpStatusException : Exception
    {
        public IResponse Response { get; }

        public HttpStatusException(
            string message,
            IResponse response) : base(message) => Response = response;
    }
}