

#if NET45
using RestClient.Net.Abstractions.Logging;
#else
#endif

namespace RestClient.Net.UnitTests
{
    public class Error
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}
