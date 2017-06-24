using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CF.RESTClientDotNet
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        public async Task<object> DeserializeAsync(byte[] binary, Type type)
        {
            var markup = await EncodeStringAsync(binary);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject(markup, type));
        }

        public async Task<T> DeserializeAsync<T>(byte[] binary)
        {
            var markup = await EncodeStringAsync(binary);
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(markup));
        }

        public async Task<byte[]> SerializeAsync<T>(T value)
        {
            var json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(value));
            var binary = await Task.Factory.StartNew(() => Encoding.UTF8.GetBytes(json));
            return binary;
        }

        public async Task<string> EncodeStringAsync(byte[] bytes)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        public async Task<byte[]> DecodeStringAsync(string theString)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetBytes(theString));
        }    
	}
}
