#if !NET45

using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public interface IGetString
    {
        IClient Client { get; }
        Task<string?> GetStringAsync();
    }

}
#endif