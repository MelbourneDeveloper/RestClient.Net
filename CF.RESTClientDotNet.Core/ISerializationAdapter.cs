using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public partial interface ISerializationAdapter
    {
        Task<string> SerializeAsync<T>(object value);
        Task<T> DeserializeAsync<T>(string markup);
        Task<string> EncodeStringAsync(byte[] value);
        Task<byte[]> DecodeStringAsync(string value);
    }
}
