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
}
