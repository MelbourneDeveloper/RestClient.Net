using System;

namespace RestClient.Net.Abstractions
{
    public interface IResponse
    {
        IHeadersCollection Headers { get; set; }
        HttpRequestMethod HttpRequestMethod { get; set; }
        bool IsSuccess { get; }
        Uri? RequestUri { get; set; }
        int StatusCode { get; set; }

        byte[] GetResponseData();
    }

    public interface IResponse<TResponseBody> : IResponse
    {
        TResponseBody Body { get; set; }
    }
}