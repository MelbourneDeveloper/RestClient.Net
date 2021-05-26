#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace RestClient.Net.UnitTests
{
#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Error
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
    }
}
