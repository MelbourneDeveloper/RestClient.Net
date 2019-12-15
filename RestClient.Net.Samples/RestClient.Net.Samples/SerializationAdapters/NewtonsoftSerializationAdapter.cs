using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        #region Implementation
        public async Task<T> DeserializeAsync<T>(byte[] data)
        {
            var markup = Encoding.UTF8.GetString(data);

            object markupAsObject = markup;

            if (typeof(T) == typeof(string))
            {
                return (T)markupAsObject;
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<T>(markup));
        }

        public async Task<byte[]> SerializeAsync<T>(T value)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(value));
            var binary = await Task.Run(() => Encoding.UTF8.GetBytes(json));
            return binary;
        }
        #endregion
    }
}
