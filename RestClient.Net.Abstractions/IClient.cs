using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IClient
    {
        ISerializationAdapter SerializationAdapter { get; }
        Task<RestResponseBase<TResponseBody>> SendAsync<TResponseBody, TRequestBody>(RestRequest<TRequestBody> restRequest);
        IHeadersCollection DefaultRequestHeaders { get; }
        TimeSpan Timeout { get; set; }
    }
}