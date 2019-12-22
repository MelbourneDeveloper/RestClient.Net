using RestClientDotNet.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestClientDotNet
{
    public class BinaryDataContractSerializationAdapter : ISerializationAdapter
    {
        #region Public Static Properties
        public static List<Type> KnownDataContracts { get; } = new List<Type>();
        #endregion

        #region Public Methods
        public async Task<object> DeserializeAsync(byte[] binary, Type type)
        {
            var serializer = new DataContractSerializer(type, KnownDataContracts);
            using (var stream = new MemoryStream(binary))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                return serializer.ReadObject(reader);
            }
        }

        public async Task<T> DeserializeAsync<T>(byte[] binary)
        {
            return (T)await DeserializeAsync(binary, typeof(T));
        }

        public async Task<byte[]> SerializeAsync<T>(T value)
        {
            var serializer = new DataContractSerializer(typeof(T), KnownDataContracts);
            var stream = new MemoryStream();
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(writer, value);
            }
            return stream.ToArray();
        }

        public async Task<string> EncodeStringAsync(byte[] bytes)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        public async Task<byte[]> DecodeStringAsync(string theString)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetBytes(theString));
        }

        #endregion
    }
}
