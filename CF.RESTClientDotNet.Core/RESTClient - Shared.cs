namespace CF.RESTClientDotNet
{
    public partial class RESTClient
    {
        #region Public Properties 
        public int TimeOutMilliseconds { get; set; } = 10000;
        public bool ReadToEnd { get; set; } = true;
        public static ISerializationAdapter SerializationAdapter { get; set; }
        #endregion
    }
}
