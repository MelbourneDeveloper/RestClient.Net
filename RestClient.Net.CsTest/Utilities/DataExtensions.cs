namespace RestClient.Net.CsTest.Utilities;

/// <summary>
/// Provides extension methods for data manipulation.
/// </summary>
internal static class DataExtensions
{
    /// <summary>
    /// Splits the byte array into chunks of the specified size.
    /// </summary>
    /// <param name="data">The byte array to be split.</param>
    /// <param name="chunkSize">The size of each chunk.</param>
    /// <returns>An enumerable collection of byte arrays, each representing a chunk of the original data.</returns>
    public static IEnumerable<byte[]> SplitIntoChunks(this byte[] data, int chunkSize)
    {
        for (var i = 0; i < data.Length; i += chunkSize)
        {
            yield return data.Skip(i).Take(chunkSize).ToArray();
        }
    }
}
