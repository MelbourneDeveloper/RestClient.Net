using RestClient.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

#pragma warning disable CA1002 // Do not expose generic lists

namespace RestClient.Net
{
    public class BinaryDataContractSerializationAdapter : ISerializationAdapter
    {
        private readonly List<Type> knownDataContracts;

        public BinaryDataContractSerializationAdapter(List<Type> knownDataContracts) => this.knownDataContracts = knownDataContracts;

        #region Public Methods
        public TResponseBody Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection? responseHeaders)
        {
            if (responseData == null) throw new ArgumentNullException(nameof(responseData));

            var serializer = new DataContractSerializer(typeof(TResponseBody), knownDataContracts);
            using var stream = new MemoryStream(responseData);
            using var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max);
            return (TResponseBody)serializer.ReadObject(reader);
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var serializer = new DataContractSerializer(typeof(TRequestBody), knownDataContracts);
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
