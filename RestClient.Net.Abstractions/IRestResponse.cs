namespace RestClientDotNet.Abstractions
{
    public interface IRestResponse<TBody> : IRestResponse
    {
        TBody Body { get; }
    }

    public interface IRestResponse
    {
        IRestHeadersCollection Headers { get; }
        int StatusCode { get; }
    }
}