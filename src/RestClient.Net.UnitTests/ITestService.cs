using RestClient.Net.Abstractions;
using System.Threading.Tasks;

namespace RestClient.Net.UnitTests
{
    public interface ITestService
    {
        IClient Client { get; }
        Task<TestThing?> GetTestThingAsync();
    }
}