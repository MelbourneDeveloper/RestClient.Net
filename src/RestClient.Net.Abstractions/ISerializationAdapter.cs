namespace RestClient.Net.Abstractions
{
    public interface ISerializationAdapter
    {
        /// <summary>
        /// Takes an object of Type T and converts it to binary data for the Http Request
        /// </summary>
        byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders);

        /// <summary>
        /// Takes binary data from the Http Response and converts it to an object of type T
        /// </summary>
        TResponseBody Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection responseHeaders);
    }
}
