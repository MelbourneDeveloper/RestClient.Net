using System;

namespace RestClientDotNet.Abstractions
{
    public class HttpStatusException : Exception
    {
        public IRestResponse RestResponse { get; }

        public HttpStatusException(string message, IRestResponse restResponse) : base(message)
        {
            RestResponse = restResponse;
        }
    }
}