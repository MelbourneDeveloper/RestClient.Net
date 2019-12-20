using RestClientDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Net.CoreSample.SerializationAdapters
{
    public class ProtobufSerializationAdapter : ISerializationAdapter
    {
        public Task<byte[]> SerializeAsync<T>(T value)
        {

        }

        /// <summary>
        /// Takes binary data and converts it to an object of type T
        /// </summary>
        public Task<T> DeserializeAsync<T>(byte[] data)
        {

        }
    }
}
