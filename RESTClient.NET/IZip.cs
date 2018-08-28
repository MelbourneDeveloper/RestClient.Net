namespace RestClientDotNet
{
    public interface IZip
    {
        byte[] Unzip(byte[] compressedData);
    }
}
