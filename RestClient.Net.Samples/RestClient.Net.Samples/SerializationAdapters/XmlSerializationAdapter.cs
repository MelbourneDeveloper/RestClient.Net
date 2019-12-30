using RestClientDotNet.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestClientDotNet
{
    public class XmlSerializationAdapter : ISerializationAdapter
    {
        #region Public Methods

        public TResponseBody Deserialize<TResponseBody>(byte[] data, IRestHeadersCollection responseHeaders)
        {
            var serializer = new XmlSerializer(typeof(TResponseBody));
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return (TResponseBody)serializer.Deserialize(stream);
            }
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IRestHeadersCollection requestHeaders)
        {

            var serializer = new XmlSerializer(typeof(TRequestBody));
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            {
                serializer.Serialize(writer, value);
                var streamReader = new StreamReader(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var markup = streamReader.ReadToEnd();

                //TODO: This is unnecessary. The writer/reader should be doing this in binary and not as a string
                return Encoding.UTF8.GetBytes(markup);
            }
        }
        #endregion
    }
}
