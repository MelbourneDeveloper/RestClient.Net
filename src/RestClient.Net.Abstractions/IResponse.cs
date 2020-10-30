using System;

namespace RestClient.Net.Abstractions
{
    public interface IResponse
    {
        IHeadersCollection Headers { get; }
        HttpRequestMethod HttpRequestMethod { get; }
        bool IsSuccess { get; }
        Uri? RequestUri { get; }
        int StatusCode { get; }
        byte[] GetResponseData();
    }

    public interface IResponse<TResponseBody> : IResponse
    {
        TResponseBody Body { get; }
    }
}