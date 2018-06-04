using System.Text;

namespace CF.RESTClientDotNet
{
    public abstract  class RESTClientSerializationAdapterBase
    {
        #region Public Properties
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        #endregion

        #region Protected Methods
        protected string GetMarkup(byte[] data)
        {
            return Encoding.GetString(data);
        }
        #endregion
    }
}
