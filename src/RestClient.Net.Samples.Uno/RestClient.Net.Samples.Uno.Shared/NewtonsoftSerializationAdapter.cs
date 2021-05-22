using Newtonsoft.Json;
using System.Text;

namespace RestClient.Net
{
    public class NewtonsoftSerializationAdapter : ISerializationAdapter
    {
        #region Implementation
        public TResponseBody? Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection? responseHeaders)
        {
            //Note: on some services the headers should be checked for encoding 
            var markup = Encoding.UTF8.GetString(responseData);

            object markupAsObject = markup;

            return typeof(TResponseBody) == typeof(string) ? (TResponseBody)markupAsObject : JsonConvert.DeserializeObject<TResponseBody>(markup);
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var json = JsonConvert.SerializeObject(value);

            var binary = Encoding.UTF8.GetBytes(json);

            return binary;
        }
        #endregion
    }
}
