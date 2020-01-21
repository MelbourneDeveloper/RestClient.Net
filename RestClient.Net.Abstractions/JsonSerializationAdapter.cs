using RestClient.Net.Abstractions;
using System.Text;
using System.Text.Json;

namespace RestClient.Net
{
    /// <summary>
    /// A Serialization Adapter that uses System.Text.Json. This is the default Json SerializationAdapter. Note that its behaviour may be slightly different to Newtonsoft and this is only available on .NET Core currently.
    /// </summary>
    public class JsonSerializationAdapter : ISerializationAdapter
    {
        #region Public Properties
        public JsonSerializerOptions JsonSerializationOptions { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a Serialization Adapter that uses System.Text.Json. Note that performance can be improved by changing the serialization option PropertyNameCaseInsensitive to false 
        /// </summary>
        /// <param name="jsonSerializationOptions">Allows the default behaviour of System.Text.Json to be changed. Note that properties are insensitive by default to keep compatibility with Newtonsoft</param>
        public JsonSerializationAdapter(JsonSerializerOptions jsonSerializationOptions = null)
        {
            JsonSerializationOptions = jsonSerializationOptions ??
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
        }
        #endregion

        #region Implementation
        public TResponseBody Deserialize<TResponseBody>(byte[] data, IHeadersCollection responseHeaders)
        {
            var markup = Encoding.UTF8.GetString(data);

            object markupAsObject = markup;

            var returnValue = typeof(TResponseBody) == typeof(string) ? (TResponseBody)markupAsObject : JsonSerializer.Deserialize<TResponseBody>(markup, JsonSerializationOptions);

            return returnValue;
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {
            var json = JsonSerializer.Serialize(value, JsonSerializationOptions);

            var binary = Encoding.UTF8.GetBytes(json);

            return binary;
        }
        #endregion
    }
}

