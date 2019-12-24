using System;
using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public interface IRestClient
    {
        ISerializationAdapter SerializationAdapter { get; }
        Task<RestResponseBase<TReturn>> SendAsync<TReturn, TBody>(RestRequest<TBody> restRequest);
        IRestHeadersCollection DefaultRequestHeaders { get; }
        string DefaultContentType { get; }
        TimeSpan Timeout { get; set; }
    }
}