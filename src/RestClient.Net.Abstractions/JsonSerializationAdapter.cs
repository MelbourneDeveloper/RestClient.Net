using RestClient.Net.Abstractions;
using System;
using System.Text;
using System.Text.Json;

namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// A Serialization Adapter that uses System.Text.Json. This is the default Json SerializationAdapter. Note that its behaviour may be slightly different to Newtonsoft and this is only available on .NET Core currently.
    /// </summary>
    public class JsonSerializationAdapter : ISerializationAdapter
    {
        public static JsonSerializationAdapter Instance { get; } = new JsonSerializationAdapter();

        #region Public Properties
        public JsonSerializerOptions JsonSerializationOptions { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a Serialization Adapter that uses System.Text.Json. Note that performance can be improved by changing the serialization option PropertyNameCaseInsensitive to false 
        /// </summary>
        /// <param name="jsonSerializationOptions">Allows the default behaviour of System.Text.Json to be changed. Note that properties are insensitive by default to keep compatibility with Newtonsoft</param>
        public JsonSerializationAdapter(JsonSerializerOptions? jsonSerializationOptions = null) => JsonSerializationOptions = jsonSerializationOptions ??
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
        #endregion

        #region Implementation
        public TResponseBody Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection? responseHeaders)
        {
            if (responseData == null) throw new ArgumentNullException(nameof(responseData));

            var markup = Encoding.UTF8.GetString(responseData);

            object markupAsObject = markup;

            var returnValue = typeof(TResponseBody) == typeof(string) ? (TResponseBody)markupAsObject : JsonSerializer.Deserialize<TResponseBody>(markup, JsonSerializationOptions);

            return returnValue ?? throw new InvalidOperationException("Deserialization returned null");
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection? requestHeaders)
        {
            var json = JsonSerializer.Serialize(value, JsonSerializationOptions);

            var binary = Encoding.UTF8.GetBytes(json);

            return binary;
        }
        #endregion
    }
}

