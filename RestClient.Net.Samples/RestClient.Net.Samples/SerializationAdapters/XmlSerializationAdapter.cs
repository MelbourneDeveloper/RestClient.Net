using RestClientDotNet.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RestClientDotNet
{
    public class XmlSerializationAdapter : RestClientSerializationAdapterBase, ISerializationAdapter
    {
        #region Public Methods

        public async Task<T> DeserializeAsync<T>(byte[] binary)
        {
            var markup = Encoding.UTF8.GetString(binary);

            return await Task.Run(() =>
            {
                var bytes = Encoding.UTF8.GetBytes(markup);

                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)serializer.Deserialize(stream);
                }
            });
        }

        public async Task<byte[]> SerializeAsync<T>(T value)
        {
            var markup = await Task.Factory.StartNew(() =>
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                {
                    serializer.Serialize(writer, value);
                    var streamReader = new StreamReader(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var xml = streamReader.ReadToEnd();
                    return xml;
                }
            });

            //TODO: This is unnecessary. The writer/reader should be doing this in binary and not as a string
            return Encoding.UTF8.GetBytes(markup);
        }

        #endregion
    }
}
