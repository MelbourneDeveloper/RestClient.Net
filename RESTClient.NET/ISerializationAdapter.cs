using System;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public partial interface ISerializationAdapter
    {
        /// <summary>
        /// Takes an object of Type T and converts it to binary data
        /// </summary>
        Task<byte[]> SerializeAsync<T>(T value);

        /// <summary>
        /// Takes an object of the type specified and converts it to binary data
        /// </summary>
        Task<object> DeserializeAsync(byte[] binary, Type type);

        /// <summary>
        /// Takes binary data and converts it to an object of type T
        /// </summary>
        Task<T> DeserializeAsync<T>(byte[] binary);

        /// <summary>
        /// Converts raw binary in to a string
        /// Note: This is necessary for the purpose of converting raw binary to strings for the purpose of building Urls
        /// </summary>
        Task<string> EncodeStringAsync(byte[] value);

        /// <summary>
        /// Converts a string in to raw binary
        /// Note: This is necessary when a request includes text data in the body
        /// </summary>
        Task<byte[]> DecodeStringAsync(string value);
    }
}
