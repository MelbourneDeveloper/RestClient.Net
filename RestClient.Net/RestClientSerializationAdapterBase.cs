using System.Text;

namespace RestClientDotNet
{
    public abstract class RestClientSerializationAdapterBase
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
