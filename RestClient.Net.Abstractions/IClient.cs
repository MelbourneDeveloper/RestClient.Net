using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IClient
    {
        ISerializationAdapter SerializationAdapter { get; }
        Task<Response<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(Request<TRequestBody> restRequest);
        IHeadersCollection DefaultRequestHeaders { get; }
        TimeSpan Timeout { get; set; }
        Uri BaseUri { get; set; }
    }
}