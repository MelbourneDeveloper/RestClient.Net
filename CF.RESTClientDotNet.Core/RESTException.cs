using System;

namespace CF.RESTClientDotNet
{
    public class RESTException : Exception
    {
        public object Error { get; set; }
        public string ResultText { get; set; }

        public RESTException(object error, string resultText, string message, Exception innerException) : base(message, innerException)
        {
            Error = error;
            ResultText = resultText;
        }
    }
}
