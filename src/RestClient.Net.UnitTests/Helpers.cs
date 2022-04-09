using System.Net.Http;
using System.Reflection;

namespace RestClient.Net.UnitTests
{
    public static class TestHelpers
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        public static readonly FieldInfo HttpClientHandlerField = typeof(HttpMessageInvoker).GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
        public static readonly FieldInfo HttpClientDisposedField = typeof(HttpClient).GetField("_disposed", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore CS8601 // Possible null reference assignment.

    }
}
