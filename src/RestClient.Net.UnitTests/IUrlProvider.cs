#if !NET45


namespace RestClient.Net.UnitTests
{
    public interface IUrlProvider
    {
#pragma warning disable CA1055 // URI-like return values should not be strings
        string GetUrl();
#pragma warning restore CA1055 // URI-like return values should not be strings
    }
}
#endif