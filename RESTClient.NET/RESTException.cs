using System;
using System.Net;

namespace CF.RESTClientDotNet
{
    public class RESTException : Exception
    {
        public object Error { get; }
        public byte[] ResultData { get; }
        public HttpStatusCode HttpStatusCode { get; }

        public RESTException(object error, byte[] resultData, string message, HttpStatusCode httpStatusCode) : base(message)
        {
            Error = error;
            ResultData = resultData;
            HttpStatusCode = httpStatusCode;
        }
    }
}
