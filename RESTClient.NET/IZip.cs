namespace CF.RESTClientDotNet
{
    public interface IZip
    {
        byte[] Unzip(byte[] compressedData);
    }
}
