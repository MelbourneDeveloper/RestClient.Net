using Google.Protobuf;
using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net
{
    public class ProtobufSerializationAdapter : ISerializationAdapter
    {
        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
            => value != null ?
            !((IMessage)value is IMessage message) ? throw new Exception("The object is not a Google Protobuf Message") : message.ToByteArray() : throw new ArgumentNullException(nameof(value));

        public TResponseBody Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection? responseHeaders)
        {
            if (responseData == null) throw new ArgumentNullException(nameof(responseData));
            var messageType = typeof(TResponseBody);
            var parserProperty = messageType.GetProperty("Parser");
            if (parserProperty == null) throw new Exception("Could not get Parser");
            var parser = parserProperty.GetValue(parserProperty);
            var parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
            if (parseFromMethod == null) throw new Exception("Could not get ParseFrom method");
            var parsedObject = parseFromMethod.Invoke(parser, new object[] { responseData });
            return (TResponseBody)parsedObject;
        }
    }
}
