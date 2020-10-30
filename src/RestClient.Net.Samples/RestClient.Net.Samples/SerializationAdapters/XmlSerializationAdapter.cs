﻿using RestClient.Net.Abstractions;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RestClient.Net
{
    public class XmlSerializationAdapter : ISerializationAdapter
    {
        #region Public Methods

        public TResponseBody Deserialize<TResponseBody>(byte[] responseData, IHeadersCollection? responseHeaders)
        {
            if (responseData == null) throw new ArgumentNullException(nameof(responseData));

            var serializer = new XmlSerializer(typeof(TResponseBody));
            using var stream = new MemoryStream();
            stream.Write(responseData, 0, responseData.Length);
            _ = stream.Seek(0, SeekOrigin.Begin);
            return (TResponseBody)serializer.Deserialize(stream);
        }

        public byte[] Serialize<TRequestBody>(TRequestBody value, IHeadersCollection requestHeaders)
        {

            var serializer = new XmlSerializer(typeof(TRequestBody));
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            serializer.Serialize(writer, value);
            var streamReader = new StreamReader(memoryStream);
            _ = memoryStream.Seek(0, SeekOrigin.Begin);
            var markup = streamReader.ReadToEnd();

            //TODO: This is unnecessary. The writer/reader should be doing this in binary and not as a string
            return Encoding.UTF8.GetBytes(markup);
        }
        #endregion
    }
}
