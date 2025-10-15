using System.Net;

namespace RestClient.Net.CsTest.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1812

internal sealed class MyErrorModel
{
    public string Message { get; set; } = "";
    public HttpStatusCode StatusCode { get; set; }
}
