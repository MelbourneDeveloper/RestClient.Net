using CF.RESTClientDotNet;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CF.RESTClientDotNet
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        public async Task<byte[]> DecodeStringAsync(string theString)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetBytes(theString));
        }

        public async Task<object> DeserializeAsync(string markup, Type type)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject(markup, type));
        }

        public async Task<T> DeserializeAsync<T>(string markup)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(markup));
        }

        public async Task<string> EncodeStringAsync(byte[] bytes)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        public async Task<string> SerializeAsync<T>(T value)
        {
            return await Task.Factory.StartNew(() => JsonConvert.SerializeObject(value));
        }
    }
}
