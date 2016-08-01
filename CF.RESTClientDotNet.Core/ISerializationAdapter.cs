using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public partial interface ISerializationAdapter
    {
        Task<string> SerializeAsync(object objectToSerialize);
        Task<T> DeserializeAsync<T>(string markup);
        Task<string> EncodeStringAsync(byte[] bytes);
        Task<byte[]> DecodeStringAsync(string theString);
    }
}
