using System.Threading.Tasks;
using Urls;

namespace RestClient.Net
{
    /// <summary>
    /// Dependency Injection abstraction for Rest Clients. Use the CreateClient delegate to create an IClient when more than one is needed for an application.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Sends a strongly typed request to the server and waits for a strongly typed response
        /// </summary>
        /// <typeparam name="TResponseBody">The expected type of the response body</typeparam>
        /// <param name="request">The request that will be translated to a HTTP request</param>
        /// <returns>The response as the strong type specified by TResponseBody /></returns>
        /// <typeparam name="TRequestBody"></typeparam>
        Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(IRequest<TRequestBody> request);

        /// <summary>
        /// Default headers to be sent with HTTP requests
        /// </summary>
        IHeadersCollection DefaultRequestHeaders { get; }

        /// <summary>
        /// Base Url for the client. Any resources specified on requests will be relative to this.
        /// </summary>
        AbsoluteUrl BaseUrl { get; }
    }
}