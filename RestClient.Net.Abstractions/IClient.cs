using System;
using System.Threading.Tasks;

namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// Dependency Injection abstraction for rest clients. Use the IClientFactory abstraction when more than one client is needed for an application.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Adapter for serialization/deserialization of http body data
        /// </summary>
        ISerializationAdapter SerializationAdapter { get; }

        /// <summary>
        /// Sends a strongly typed request to the server and waits for a strongly typed response
        /// </summary>
        /// <typeparam name="TResponseBody">The expected type of the response body</typeparam>
        /// <typeparam name="TRequestBody">The type of the request body if specified</typeparam>
        /// <param name="request">The request that will be translated to a http request</param>
        /// <returns></returns>
        Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(Request<TRequestBody> request);

        /// <summary>
        /// Default headers to be sent with http requests
        /// </summary>
        IHeadersCollection DefaultRequestHeaders { get; }

        /// <summary>
        /// Default timeout for http requests
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Base Uri for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        Uri BaseUri { get; set; }

        /// <summary>
        /// Name of the client
        /// </summary>
        string Name { get; }
    }
}