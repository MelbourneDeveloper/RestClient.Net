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
            : base(string.Format("The server failed to respond after waiting for {0} seconds. Please ensure you have network connectivity and try again later.", timeoutSeconds))
        {
            _TimeoutSeconds = timeoutSeconds;
        }
    }
}