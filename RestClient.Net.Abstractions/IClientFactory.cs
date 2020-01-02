namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// Dependency Injection abstraction for creating and managing rest clients. Use this abstraction when more than one rest client is needed for the application.
    /// </summary>
    public interface IClientFactory
    {
        /// <summary>
        /// Gets or creates an IClient based on the specified name. A factory may keep a single instance, an instance per name, or many instances per name. Check the implementation for details.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An IClient instance</returns>
        IClient CreateClient(string name);
    }
}
