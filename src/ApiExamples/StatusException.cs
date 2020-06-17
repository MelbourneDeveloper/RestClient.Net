using System;
using System.Net;

namespace ApiExamples
{
    public class StatusException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public StatusException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
