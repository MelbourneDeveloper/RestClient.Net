using System.Text;

namespace CF.RESTClientDotNet
{
    public static class Extensions
    {
        public static string EncodeString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] DecodeString(this string theString)
        {
            return Encoding.UTF8.GetBytes(theString);
        }
    }
}
