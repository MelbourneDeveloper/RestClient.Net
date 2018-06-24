using System;
using System.Net;

namespace CF.RESTClientDotNet
{
    public class HttpStatusException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public byte[] ErrorData { get; }

        public HttpStatusException(string message, HttpStatusCode statusCode, byte[] errorData) : base(message)
        {
            StatusCode = statusCode;
            ErrorData = errorData;
        }
    }
}