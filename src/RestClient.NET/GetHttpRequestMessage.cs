using Microsoft.Extensions.Logging;
using RestClient.Net.Abstractions;
using System;
using System.Net.Http;

namespace RestClient.Net
{
    public delegate HttpRequestMessage GetHttpRequestMessage(IRequest request, ILogger logger, Uri? baseUri);
}
