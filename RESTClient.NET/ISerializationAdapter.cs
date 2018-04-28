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
        /// Takes binary data and converts it to an object of type T
        /// </summary>
        Task<T> DeserializeAsync<T>(byte[] binary);
    }
}
