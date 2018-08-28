using System;
using System.Text;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public partial interface ISerializationAdapter
    {
        Encoding Encoding { get; }

        /// <summary>
        /// Takes an object of Type T and converts it to binary data
        /// </summary>
        Task<byte[]> SerializeAsync<T>(T value);

        /// <summary>
        /// Takes binary data and converts it to an object of type T
        /// </summary>
        Task<T> DeserializeAsync<T>(byte[] binary);

        Task<object> DeserializeAsync(byte[] data, Type type);
    }
}
