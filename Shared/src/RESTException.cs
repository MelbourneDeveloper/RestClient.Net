using System;

namespace CF.RESTClientDotNet
{
    public class RESTException : Exception
    {
        public object Error { get; set; }
        public byte[] ResultData { get; set; }

        public RESTException(object error, byte[] resultData, string message, Exception innerException) : base(message, innerException)
        {
            Error = error;
            ResultData = resultData;
        }
    }
}
