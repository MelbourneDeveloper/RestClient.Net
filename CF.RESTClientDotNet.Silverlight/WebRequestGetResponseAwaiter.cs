using System;
using System.Net;

namespace CF.RESTClientDotNet
{
    public class WebRequestGetResponseAwaiter
    {
        #region Events
        public event EventHandler GetResponseCompleted;
        #endregion

        #region Public Properties
        public IAsyncResult Result{get; private set;}
        public WebRequest WebRequest { get; private set; }
        #endregion

        #region Constructor
        public WebRequestGetResponseAwaiter(WebRequest webRequest)
        {
            WebRequest = webRequest;
        }
        #endregion

        #region Public Methods
        public void GetResponseAsync()
        {
            AsyncCallback callBack = (asyncResult) =>
            {
                Result = asyncResult;
                GetResponseCompleted?.Invoke(this, new EventArgs());
            };

            WebRequest.BeginGetResponse(callBack, WebRequest);
        }
        #endregion
    }
}
