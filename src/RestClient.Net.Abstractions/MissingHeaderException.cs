using System;

namespace RestClient.Net
{
    public class MissingHeaderException : Exception
    {
        public bool IsRequest { get; }

        public MissingHeaderException(
            string message,
            bool isRequest,
            Exception innerException) :
            base(message, innerException) => IsRequest = isRequest;
    }
}