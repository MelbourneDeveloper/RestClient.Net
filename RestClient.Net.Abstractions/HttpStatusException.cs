using System;

namespace RestClientDotNet.Abstractions
{
    public class HttpStatusException : Exception
    {
        public RestResponseBase RestResponse { get; }

        public HttpStatusException(string message, RestResponseBase restResponse) : base(message)
        {
            RestResponse = restResponse;
        }
    }
}