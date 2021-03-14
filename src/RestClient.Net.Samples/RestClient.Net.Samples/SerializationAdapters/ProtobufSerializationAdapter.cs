using Google.Protobuf;
using RestClient.Net.Abstractions;
using System;

namespace RestClient.Net
{
    public class ProtobufSerializationAdapter : ISerializationAdapter
    {
        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
            => value != null ?
            (IMessage)value is not IMessage message ? throw new InvalidOperationException("The object is not a Google Protobuf Message") : message.ToByteArray() : throw new ArgumentNullException(nameof(value));

        public TResponseBody Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection? responseHeaders)
        {
            if (responseData == null) throw new ArgumentNullException(nameof(responseData));
            var messageType = typeof(TResponseBody);
            var parserProperty = messageType.GetProperty("Parser");
            if (parserProperty == null) throw new InvalidOperationException("Could not get Parser");
            var parser = parserProperty.GetValue(parserProperty);
            var parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
            if (parseFromMethod == null) throw new InvalidOperationException("Could not get ParseFrom method");
            var parsedObject = parseFromMethod.Invoke(parser, new object[] { responseData });

            return parsedObject == null ? throw new InvalidOperationException("Parsed object is null") : (TResponseBody)parsedObject;
        }
    }
}
