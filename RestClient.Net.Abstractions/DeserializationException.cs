using System;

namespace RestClientDotNet.Abstractions
{
    public class DeserializationException : Exception
    {
        private readonly byte[] _responseData;
        public IRestClient RestClient { get; }

        public DeserializationException(
            string message,
            byte[] responseData,
            IRestClient restClient,
            Exception innerException) : base(message, innerException)
        {
            _responseData = responseData;
            RestClient = restClient;
        }

        public byte[] GetResponseData()
        {
            return _responseData;
        }
    }
}
