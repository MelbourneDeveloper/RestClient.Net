using Google.Protobuf;
using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net
{
    public class ProtobufSerializationAdapter : ISerializationAdapter
    {
        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders) => !((IMessage)value is IMessage message) ? throw new Exception("The object is not a Google Protobuf Message") : message.ToByteArray();

        public TResponseBody Deserialize<TResponseBody>(Response response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            var messageType = typeof(TResponseBody);
            var parserProperty = messageType.GetProperty("Parser");
            var parser = parserProperty.GetValue(parserProperty);
            var parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
            var parsedObject = parseFromMethod.Invoke(parser, new object[] { response.GetResponseData() });
            return (TResponseBody)parsedObject;
        }
    }
}
