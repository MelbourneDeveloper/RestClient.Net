namespace RestClientDotNet.Abstractions
{
    public interface IClientFactory
    {
        IClient CreateClient(string name);
    }
}
