using Urls;

namespace RestClient.Net.Abstractions
{
    public interface IResponse
    {
        IHeadersCollection Headers { get; }
        HttpRequestMethod HttpRequestMethod { get; }
        bool IsSuccess { get; }
        AbsoluteUrl RequestUri { get; }
        int StatusCode { get; }
        byte[] GetResponseData();
    }

    public interface IResponse<TResponseBody> : IResponse
    {
        TResponseBody Body { get; }
    }
}