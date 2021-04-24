#if !NET45

using RestClient.Net.Abstractions;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public interface IGetString
    {
        IClient Client { get; }
        Task<string> GetStringAsync();
    }

}
#endif