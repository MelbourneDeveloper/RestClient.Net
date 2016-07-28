
using System;
using System.Net;

namespace CF.RESTClientDotNet
{
    /// <summary>
    /// Represents the results of a call to a REST service
    /// </summary>
    public class RESTResponse<T> : RESTResponse
    {
        /// <summary>
        /// The deserialised object constructed from the json returned from the call
        /// </summary>
        public new T Data { get; set; }      
    }

    public class RESTResponse
    {
        /// <summary>
        /// The serialised json returned from the call
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The low level HttpWebResponse from the REST call
        /// </summary>
#if (WINDOWS_UWP)
        public WebResponse Response { get; set; }
#else
        public HttpWebResponse Response { get; set; }
#endif

        /// <summary>
        /// The exception that occurred
        /// </summary>
        public Exception Error { get; set; }
    }

}