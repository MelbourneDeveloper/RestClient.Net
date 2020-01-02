namespace RestClient.Net.Abstractions
{
    public interface ISerializationAdapter
    {
        /// <summary>
        /// Takes an object of Type T and converts it to binary data for the Http Request
        /// </summary>
        /// <typeparam name="TRequestBody">The type to be serialized from</typeparam>
        /// <param name="value">The object to be serialized</param>
        /// <param name="requestHeaders">Headers that will be sent as part of the Http Request</param>
        /// <returns></returns>
        byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders);

        /// <summary>
        /// Takes binary data from the Http Response and converts it to an object of type T
        /// </summary>
        /// <typeparam name="TResponseBody">The type to serialize to</typeparam>
        /// <param name="data">The Http Response's body data</param>
        /// <param name="responseHeaders">The headers on the Http Response from the server</param>
        /// <returns></returns>
        TResponseBody Deserialize<TResponseBody>(byte[] data, IHeadersCollection responseHeaders);
    }
}
