using System;
using System.Net.Http;

namespace RestClient.Net.UnitTests
{
    public class OverzealousHttpClientFactory
    {

        #region Fields
        private readonly Func<string, HttpClient> _getOrAddFunc;
        #endregion

        #region Constructor
        public OverzealousHttpClientFactory() : this(null)
        {
        }

        public OverzealousHttpClientFactory(Func<string, HttpClient> func)
        {
            _getOrAddFunc = func;

            if (_getOrAddFunc != null) return;
            _getOrAddFunc = name =>
            {
                return new HttpClient();
            };
        }
        #endregion

        #region Implementation
        public HttpClient CreateClient(string name) => name == null ? throw new ArgumentNullException(nameof(name)) : _getOrAddFunc.Invoke(name);
        #endregion
    }
}
