using System;

namespace CF.RESTClientDotNet
{
    /// <summary>
    /// Error that occurs when the server doesn't response in time
    /// </summary>
    public class RESTTimeoutException : Exception
    {
        private int _TimeoutSeconds;

        public int TimeoutSeconds
        {
            get { return _TimeoutSeconds; }
        }

        public RESTTimeoutException(int timeoutSeconds)
            : base($"The server failed to respond after waiting for {timeoutSeconds} seconds. Please ensure you have network connectivity and try again later.")
        {
            _TimeoutSeconds = timeoutSeconds;
        }
    }
}