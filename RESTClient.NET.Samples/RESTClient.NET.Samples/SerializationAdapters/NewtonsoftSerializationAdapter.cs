using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        #region Public Properties
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        #endregion

        #region Implementation
        public async Task<T> DeserializeAsync<T>(byte[] binary)
        {
            var markup = Encoding.GetString(binary);

            object markupAsObject = markup;

            if (typeof(T) == typeof(string))
            {
                return (T)markupAsObject;
            }

            var retVal = await Task.Run(() => JsonConvert.DeserializeObject<T>(markup));

            return retVal;
        }

        public async Task<byte[]> SerializeAsync<T>(T value)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(value));
            var binary = await Task.Run(() => Encoding.GetBytes(json));
            return binary;
        }
        #endregion
    }
}
