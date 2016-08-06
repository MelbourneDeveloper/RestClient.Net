using CF.RESTClientDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public class DataContractSerializationAdapter : ISerializationAdapter
    {
        public static List<Type> KnownDataContracts { get; } = new List<Type>();

        public async Task<byte[]> DecodeStringAsync(string theString)
        {
            return await Task.Run(() => Encoding.UTF8.GetBytes(theString));
        }

        public async Task<T> DeserializeAsync<T>(string markup)
        {
            return await Task.Factory.StartNew(() => (T)DataContractDeserializeObject(typeof(T), markup));
        }

        public async Task<string> EncodeStringAsync(byte[] bytes)
        {
            return await Task.Run(() => Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }

        public async Task<string> SerializeAsync<T>(object value)
        {
            return await Task.Factory.StartNew(() => DataContractSerializeObject(value, typeof(T)));
        }

        private static object DataContractDeserializeObject(Type sourceObjectType, string objectXml)
        {
            //Create a new DataContractSerialiser for deserialisation
            var dataContractSerializer = new DataContractSerializer(sourceObjectType, KnownDataContracts);

            //Create the second memory stream from the xml buffer
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(objectXml)))
            {

                //Get the cloned object
                return dataContractSerializer.ReadObject(memoryStream);
            }
        }


        public static string DataContractSerializeObject(object sourceObject, Type sourceObjectType)
        {
            //Create a memory stream
            using (var memoryStream = new MemoryStream())
            {
                //Create a serialiser based on the object's type
                var dataContractSerializer = new DataContractSerializer(sourceObjectType, KnownDataContracts);

                //Write the object to the memory stream
                dataContractSerializer.WriteObject(memoryStream, sourceObject);

                //Write the memory stream to the buffer
                var buffer = memoryStream.ToArray();

                //Convert the buffer to xml
                return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
        }

    }
}
