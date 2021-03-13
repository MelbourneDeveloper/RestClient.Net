#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace RestClient.Net.UnitTests
{
    public class Error
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}
