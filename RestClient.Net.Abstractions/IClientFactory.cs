namespace RestClient.Net.Abstractions
{
    public interface IClientFactory
    {
        IClient CreateClient(string name);
    }
}
