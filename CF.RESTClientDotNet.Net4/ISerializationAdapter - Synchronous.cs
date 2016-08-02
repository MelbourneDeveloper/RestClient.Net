namespace CF.RESTClientDotNet
{
    public partial interface ISerializationAdapter
    {
        string Serialize(object objectToSerialize);
        T Deserialize<T>(string markup);
        string EncodeString(byte[] bytes);
        byte[] DecodeString(string theString);
    }
}
