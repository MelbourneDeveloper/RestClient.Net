using System;

namespace RestClient.Net
{
    public class DeserializationException : Exception
    {
        private readonly byte[] responseData;

        public DeserializationException(
            string message,
            byte[] responseData,
            Exception? innerException) : base(message, innerException) => this.responseData = responseData;

        public byte[] GetResponseData() => responseData;
    }
}
