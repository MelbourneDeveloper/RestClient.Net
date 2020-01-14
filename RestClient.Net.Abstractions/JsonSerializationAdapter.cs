

using RestClient.Net.Abstractions;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestClient.Net
{
    public class JsonSerializationAdapter : ISerializationAdapter
    {
        #region Implementation
        public TResponseBody Deserialize<TResponseBody>(byte[] data, IHeadersCollection responseHeaders)
        {
            //This here is why I don't like JSON serialization. 😢
            //Note: on some services the headers should be checked for encoding 
            var markup = Encoding.UTF8.GetString(data);

            object markupAsObject = markup;

            if (typeof(TResponseBody) == typeof(string))
            {
                return (TResponseBody)markupAsObject;
            }

            return JsonSerializer.Deserialize<TResponseBody>(markup);
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var json = JsonSerializer.Serialize(value);

            //This here is why I don't like JSON serialization. 😢
            var binary = Encoding.UTF8.GetBytes(json);

            return binary;
        }
        #endregion
    }
}

