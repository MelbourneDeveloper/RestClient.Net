using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace RestClient.Net
{
    public class BinaryDataContractSerializationAdapter : ISerializationAdapter
    {
        #region Public Static Properties
        public static List<Type> KnownDataContracts { get; } = new List<Type>();
        #endregion

        #region Public Methods
        public TResponseBody Deserialize<TResponseBody>(byte[] data, IHeadersCollection responseHeaders)
        {
            var serializer = new DataContractSerializer(typeof(TResponseBody), KnownDataContracts);
            using (var stream = new MemoryStream(data))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                return (TResponseBody)serializer.ReadObject(reader);
            }
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var serializer = new DataContractSerializer(typeof(TRequestBody), KnownDataContracts);
            var stream = new MemoryStream();
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(writer, value);
            }
            return stream.ToArray();
        }
        #endregion
    }
}
