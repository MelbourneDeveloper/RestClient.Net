using System.Threading.Tasks;
using Urls;

namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// Dependency Injection abstraction for rest clients. Use the IClientFactory abstraction when more than one client is needed for an application.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Sends a strongly typed request to the server and waits for a strongly typed response
        /// </summary>
        /// <typeparam name="TResponseBody">The expected type of the response body</typeparam>
        /// <param name="request">The request that will be translated to a http request</param>
        /// <returns>The response as the strong type specified by TResponseBody /></returns>
        /// <typeparam name="TRequestBody"></typeparam>
        Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(IRequest<TRequestBody> request);

        /// <summary>
        /// Default headers to be sent with http requests
        /// </summary>
        IHeadersCollection DefaultRequestHeaders { get; }

        /// <summary>
        /// Base Uri for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        AbsoluteUrl BaseUri { get; }
    }
}