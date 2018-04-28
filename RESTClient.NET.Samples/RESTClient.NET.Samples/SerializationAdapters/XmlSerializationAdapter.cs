using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CF.RESTClientDotNet
{
    public class XmlSerializationAdapter : ISerializationAdapter
    {
        #region Public Methods
        public async Task<object> DeserializeAsync(byte[] binary, Type type)
        {
            var markup = await EncodeStringAsync(binary);

            return await Task.Factory.StartNew(() =>
            {
                var bytes = Encoding.UTF8.GetBytes(markup);

                var serializer = new XmlSerializer(type);
                using (var stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    return serializer.Deserialize(stream);
                }
            });
        }

        public async Task<T> DeserializeAsync<T>(byte[] binary)
        {
            return (T)await DeserializeAsync(binary, typeof(T));
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

        public async Task<string> EncodeStringAsync(byte[] bytes)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

      	public async Task<byte[]> DecodeStringAsync(string theString)
        {
            return await Task.Factory.StartNew(() => Encoding.UTF8.GetBytes(theString));
        }

        #endregion
    }
}
