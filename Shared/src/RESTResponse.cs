
using System;
using System.Net;

namespace CF.RESTClientDotNet
{
    public class RESTResponse
    {
        /// <summary>
        /// The serialised json returned from the call
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The low level HttpWebResponse from the REST call
        /// </summary>
        public WebResponse Response { get; set; }

        /// <summary>
        /// The exception that occurred
        /// </summary>
        public Exception Error { get; set; }
    }

}