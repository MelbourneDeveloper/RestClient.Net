using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net
{
    [Serializable]
    public class SendException : Exception
    {
        public IRequest Request { get; }

        public SendException(string message, IRequest request, Exception innerException) : base(message, innerException) => Request = request;
    }
}