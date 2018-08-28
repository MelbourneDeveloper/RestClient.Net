using System;

namespace RestClientDotNet
{
    public class DeserializationException : Exception
    {
        public byte[] ResponseData { get; private set; }

        public string Markup { get; private set; }

        public DeserializationException(byte[] responseData, string markup, Exception innerException) : base($"An error occurred while attempting to deserialize the data from the server. Markup: {markup}", innerException)
        {
            ResponseData = responseData;
            Markup = markup;
        }
    }
}
