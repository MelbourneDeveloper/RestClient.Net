namespace RestClient.Net.Abstractions
{
    /// <summary>
    /// Abstraction for compressing and decompressing data. This is used in cases where the platform does not automatically handle compression/decompression
    /// </summary>
    public interface IZip
    {
        byte[] Unzip(byte[] compressedData);
        byte[] Zip(byte uncompressedData);
    }
}
