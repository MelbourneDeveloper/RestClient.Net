using System;
using System.Collections.Generic;

namespace CF.RESTClientDotNet
{
    public partial class RESTClient
    {
        #region Public Properties 
        public int TimeoutMilliseconds { get; set; } = 10000;
        public bool ReadToEnd { get; set; } = true;
        public static ISerializationAdapter SerializationAdapter { get; set; }
        public Uri BaseUri { get; set; }
        #endregion

        #region Public Static Properties
        private static List<Type> PrimitiveTypes { get; } = new List<Type> { typeof(string), typeof(int), typeof(Guid), typeof(long), typeof(byte), typeof(char) };

        #endregion
    }
}
