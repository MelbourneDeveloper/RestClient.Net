using Google.Protobuf;
using RestClientDotNet.Abstractions;
using System;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class ProtobufSerializationAdapter : ISerializationAdapter
    {
        public async Task<byte[]> SerializeAsync<T>(T value)
        {
            var message = (IMessage)value as IMessage;           
            if (message == null) throw new Exception("The object is not a Google Protobuf Message");
            return message.ToByteArray();
        }

        /// <summary>
        /// Takes binary data and converts it to an object of type T
        /// </summary>
        public async Task<T> DeserializeAsync<T>(byte[] data)
        {
            var messageType = typeof(T);
            var parserProperty = messageType.GetProperty("Parser");
            var parser = parserProperty.GetValue(parserProperty);
            var parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
            var parsedObject = parseFromMethod.Invoke(parser,new object[] { data });
            return (T)parsedObject;
        }
    }
}
