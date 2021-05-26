using System.Collections.Generic;

namespace RestClient.Net
{
    /// <summary>
    /// Abstraction for storing and enumerating http request headers
    /// </summary>
    public interface IHeadersCollection : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        /// <summary>
        /// Gets the collection of strings belonging to a header name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<string> this[string name]
        {
            get;
        }

        /// <summary>
        /// Checks to see if the collection contains a given header
        /// </summary>
        /// <param name="name">The name of the header to check for</param>
        /// <returns>True or false</returns>
        bool Contains(string name);

        /// <summary>
        /// Lists the names of the headers in the collection
        /// </summary>
        IEnumerable<string> Names { get; }
    }
}
