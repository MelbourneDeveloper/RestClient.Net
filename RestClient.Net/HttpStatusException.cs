using RestClientDotNet.Abstractions;
using System;

namespace RestClientDotNet
{
    public class HttpStatusException : Exception
    {
        public RestResponse RestResponse { get; }

        public HttpStatusException(string message, RestResponse restResponse) : base(message)
        {
            RestResponse = restResponse;
        }
    }
}