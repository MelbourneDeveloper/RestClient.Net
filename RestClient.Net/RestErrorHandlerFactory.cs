using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;

namespace RestClientDotNet
{
    public class RestErrorHandlerFactory : IErrorHandlerFactory
    {
        private readonly ISerializationAdapter _serializationAdapter;

        public RestErrorHandlerFactory(ISerializationAdapter serializationAdapter)
        {
            _serializationAdapter = serializationAdapter;
        }

        public IErrorHandler Create(object response)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (!(response is HttpResponseMessage asdasd)) throw new ArgumentException($"{nameof(response)} must be a HttpResponseMessage");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            return new ErrorHandler(asdasd, _serializationAdapter);
        }
    }

    public class ErrorHandler : IErrorHandler
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly HttpResponseMessage _httpResponseMessage;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ISerializationAdapter _serializationAdapter;
#pragma warning restore IDE0052 // Remove unread private members

        public ErrorHandler(HttpResponseMessage httpResponseMessage, ISerializationAdapter serializationAdapter)
        {
            _httpResponseMessage = httpResponseMessage;
            _serializationAdapter = serializationAdapter;
        }

        public T GetErrorModel<T>()
        {
            throw new NotImplementedException();
        }
    }

}
