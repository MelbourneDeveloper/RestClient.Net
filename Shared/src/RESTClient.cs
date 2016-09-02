using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public partial class RESTClient
    {
        #region Fields
#if (!SILVERLIGHT)
        private Dictionary<string, string> _Headers = new Dictionary<string, string>();
#endif
        #endregion

        #region Public Properties 
        public Type ErrorType { get; set; }
        public int TimeoutMilliseconds { get; set; } = 10000;
        public bool ReadToEnd { get; set; } = true;
        public static ISerializationAdapter SerializationAdapter { get; set; }
        public Uri BaseUri { get; set; }

#if (!SILVERLIGHT)
        /// <summary>
        /// Allows headers to be sent as part of the request. Note: This it seems that this not supported in Silverlight
        /// </summary>
        public Dictionary<string, string> Headers
        {
            get
            {
                return _Headers;
            }
        }
#endif

        #endregion

        #region Public Static Properties
        private static List<Type> PrimitiveTypes { get; } = new List<Type> { typeof(string), typeof(int), typeof(Guid), typeof(long), typeof(byte), typeof(char) };

        /// <summary>
        /// The last WebResponse returned from a REST call
        /// </summary>
        public WebResponse LastRestResponse { get; private set; }
        #endregion

        #region Constructor

        public RESTClient(ISerializationAdapter serializationAdapter, Uri baseUri)
        {
            SerializationAdapter = serializationAdapter;
            BaseUri = baseUri;
        }

        #endregion

        #region Public Methods

        #region POST

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body)
        {
            return await CallAsync<TReturn, TBody, object>(body, null, HttpVerb.Post);
        }

        public async Task<RESTResponse> PostAsync<TBody, TQueryString>(TBody body, TQueryString queryString)
        {
            return await GetRESTResponse(body, queryString, HttpVerb.Post);
        }

        public async Task<TReturn> PostAsync<TReturn>(string body)
        {
            return await CallAsync<TReturn, string, object>(body, null, HttpVerb.Post);
        }

        #endregion

        #region PUT
        /// <summary>
        /// Make REST PUT call and wait for the response
        /// </summary>
        public async Task<TReturn> PutAsync<TReturn, TBody, TQueryString>(TBody body, TQueryString queryString)
        {
            //TODO: This method currently remains untested. But can be tested by uncommenting this line.");
            return await CallAsync<TReturn, TBody, TQueryString>(body, queryString, HttpVerb.Put);
        }
        #endregion

        #region GET
        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        public async Task<TReturn> GetAsync<TReturn, TQueryString>(TQueryString queryString)
        {
            return await CallAsync<TReturn, object, TQueryString>(null, queryString, HttpVerb.Get);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        /// 
        public async Task<TReturn> GetAsync<TReturn>()
        {
            return await CallAsync<TReturn, object, object>(null, null, HttpVerb.Get);
        }

        public async Task<RESTResponse> GetAsync()
        {
            return await GetRESTResponse(null, null, HttpVerb.Get);
        }
        #endregion

        #endregion

        #region Private Methods

        #region Base Calls

        /// <summary>
        /// Make REST call and wait for the response
        /// </summary>
        private async Task<WebResponse> GetWebResponse(object body, object queryString, HttpVerb verb)
        {
            try
            {
                //Get the Http Request object
                var request = await GetRequestAsync(body, queryString, verb);

                //Get the response from the server
                return await request.GetResponseAsync();
            }
            catch (WebException wex)
            {
                using (var streamReader = new StreamReader(wex.Response.GetResponseStream()))
                {
                    object error = null;
                    var responseText = await GetDataFromResponseStreamAsync(wex.Response);
                    if (ErrorType != null)
                    {
                        error = await SerializationAdapter.DeserializeAsync(responseText, ErrorType);
                    }

                    throw new RESTException(error, responseText, "The REST call returned an error. Please see Error property for details", wex);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The request timed out"))
                {
                    //The REST call timed out so throw this exception
                    throw new RESTTimeoutException(TimeoutMilliseconds / 1000);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<TReturn> CallAsync<TReturn, TBody, TQueryString>(TBody body, TQueryString queryString, HttpVerb verb)
        {
            var restResponse = await GetRESTResponse(body, queryString, verb);

            var retVal = await DeserialiseResponseAsync<TReturn>(restResponse);

            return retVal;
        }

        private async Task<RESTResponse> GetRESTResponse(object body, object queryString, HttpVerb verb)
        {
            var webResponse = await GetWebResponse(body, queryString, verb);

            var restResponse = new RESTResponse();
            restResponse.Response = webResponse;
            restResponse.Data = await GetDataFromResponseStreamAsync(webResponse);
            return restResponse;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a HttpWebRequest so that the REST call can be made with it
        /// </summary>
        private async Task<WebRequest> GetRequestAsync<TBody, TQueryString>(TBody body, TQueryString queryString, HttpVerb verb)
        {
            var theUri = BaseUri;

            if (queryString != null)
            {
                string queryStringText;
                if (PrimitiveTypes.Contains(queryString.GetType()))
                {
                    //No need to serialize
                    queryStringText = queryString.ToString();
                }
                else
                {
                    queryStringText = await SerializationAdapter.SerializeAsync(queryString);
                    queryStringText = Uri.EscapeDataString(queryStringText);
                }

                theUri = new Uri($"{ theUri.AbsoluteUri}/{queryStringText}");
            }

            //Create the web request
            var retVal = (HttpWebRequest)WebRequest.Create(theUri);

            //Switch on the verb
            switch (verb)
            {
                case HttpVerb.Post:
                    retVal.Method = "POST";
                    break;
                case HttpVerb.Get:
                    retVal.Method = "GET";
                    break;
                case HttpVerb.Put:
                    retVal.Method = "PUT";
                    break;
                default:
                    throw new NotImplementedException();
            }

            //We're always going to use json
#if (!SILVERLIGHT)
            retVal.ContentType = "application/json";
#endif

            if (body != null && new List<HttpVerb> { HttpVerb.Post, HttpVerb.Put }.Contains(verb))
            {
                //Set the body of the POST/PUT
                string markup = null;
                var bodyAsString = body as string;
                if (bodyAsString != null)
                {
                    //The body is already a string
                    markup = bodyAsString;
                }
                else
                {
                    //Serialise the data
                    markup = await SerializationAdapter.SerializeAsync(body);
                }

                //Get the json as a byte array
                var markupBuffer = await SerializationAdapter.DecodeStringAsync(markup);

                using (var requestStream = await retVal.GetRequestStreamAsync())
                {
                    requestStream.Write(markupBuffer, 0, markupBuffer.Length);
                }
            }

#if (!SILVERLIGHT)
            foreach (var key in Headers?.Keys)
            {
                retVal.Headers[key] = Headers[key];
            }
#endif

            //TODO: Reimplement
            //#if (!NETFX_CORE && !SILVERLIGHT)
            //            retVal.Timeout = timeOutMilliseconds;
            //#endif

            //Return the request
            return retVal;
        }



        /// <summary>
        /// Given the response from the REST call, return the string(
        /// </summary>
        private async Task<string> GetDataFromResponseStreamAsync(WebResponse response)
        {
            var responseStream = response.GetResponseStream();
            byte[] responseBuffer = null;

            if (!ReadToEnd)
            {
                if (responseStream.Length == -1)
                {
                    throw new Exception("An error occurred while getting data from the server. Please contact support");
                }

                //Read the stream in to a buffer
                responseBuffer = new byte[responseStream.Length];

                //Read from the stream (complete)
                var responseLength = await responseStream.ReadAsync(responseBuffer, 0, (int)responseStream.Length);
            }
            else
            {
                var reader = new StreamReader(responseStream);
                return await reader.ReadToEndAsync();
            }

            //Convert the response from bytes to json string 
            return await SerializationAdapter.EncodeStringAsync(responseBuffer);
        }

        /// <summary>
        /// Turn a non-generic RESTResponse in to a generic one. 
        /// </summary>
        private async Task<ReturnT> DeserialiseResponseAsync<ReturnT>(RESTResponse response)
        {
            ReturnT retVal = default(ReturnT);

            if (typeof(ReturnT) == typeof(string))
            {
                retVal = (ReturnT)(object)response.Data;
            }
            else
            {
                //Deserialise the json to the generic type
                retVal = await SerializationAdapter.DeserializeAsync<ReturnT>(response.Data);
            }

            //Set the HttpWebResponse
            LastRestResponse = response.Response;

            return retVal;
        }

        #endregion

        #endregion
    }
}