using RestClientDotNet.Abstractions;
using System;

namespace RestClientDotNet
{
    [Serializable]
    public class SendException<TRequestBody> : Exception
    {
        public RestRequest<TRequestBody> RestRequest { get; }

        public SendException(string message, RestRequest<TRequestBody> restRequest, Exception ex) : base(message, ex)
        {
            RestRequest = restRequest;
        }
    }
}