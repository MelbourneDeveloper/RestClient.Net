using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public partial interface ISerializationAdapter
    {
        Task<string> SerializeAsync(object value);
        Task<T> DeserializeAsync<T>(string markup);
        Task<string> EncodeStringAsync(byte[] value);
        Task<byte[]> DecodeStringAsync(string value);
    }
}
