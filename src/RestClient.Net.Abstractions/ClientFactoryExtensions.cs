using System;

namespace RestClient.Net.Abstractions
{
    public static class ClientFactoryExtensions
    {
        /// <summary>
        /// Creates a client by the name of RestClient. Note: The factory will never create more than one IClient if this extension is used. The Client will act as a singleton, and only one HttpClient name will be used for the HttpClient factory. This should only be used for simple scenarios where the BaseUri is always the same and headers can be safely reused. 
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <returns>An instance of IClient that may be recycled from a previous call</returns>
        public static IClient CreateClient(this CreateClient clientFactory) =>
            //Use 'RestClient' by default because if this is null, the dictionary fails,
            //If this is random, then many clients will get created
            CreateClient(clientFactory, "RestClient", null);

        public static IClient CreateClient(this CreateClient clientFactory, string name, Uri? baseUri)
        {
            if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
            var client = clientFactory(name);
            client.BaseUri = baseUri;
            return client;
        }
    }
}
