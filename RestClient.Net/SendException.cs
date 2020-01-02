using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net
{
    [Serializable]
    public class SendException<TRequestBody> : Exception
    {
        public Request<TRequestBody> Request { get; }

        public SendException(string message, Request<TRequestBody> request, Exception innerException) : base(message, innerException)
        {
            Request = request;
        }
    }
}