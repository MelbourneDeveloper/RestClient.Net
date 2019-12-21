namespace RestClientDotNet.Abstractions
{
    public interface IRestResponse
    {
        IRestHeadersCollection Headers { get; }
        int StatusCode { get; }
        object UnderlyingResponse { get; }
    }
}