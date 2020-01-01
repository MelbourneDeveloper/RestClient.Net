using System;

namespace RestClient.Net.Abstractions
{
    public class DeserializationException : Exception
    {
        private readonly byte[] _responseData;
        public IClient Client { get; }

        public DeserializationException(
            string message,
            byte[] responseData,
            IClient client,
            Exception innerException) : base(message, innerException)
        {
            _responseData = responseData;
            Client = client;
        }

        public byte[] GetResponseData()
        {
            return _responseData;
        }
    }
}
