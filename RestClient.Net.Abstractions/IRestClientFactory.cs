namespace RestClientDotNet.Abstractions
{
    public interface IRestClientFactory
    {
        IRestClient CreateRestClient();
    }
}
