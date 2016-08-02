using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public partial class RESTClient
    {
        #region Constructor

        public RESTClient(ISerializationAdapter serializationAdapter, Uri baseUri)
        {
            SerializationAdapter = serializationAdapter;
            BaseUri = baseUri;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public async Task<RESTResponse<T>> PostAsync<T, T1, T2>(Uri baseUri, T1 body, T2 queryString)
        {
            return await CallPostAsync<T, T1, T2>(BaseUri, body, queryString, TimeoutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public async Task<WebResponse> PostAsync<T, T1, T2>(T1 body, T2 queryString)
        {
            return await CallAsync(BaseUri, body, queryString, HttpVerb.Post, TimeoutMilliseconds);
        }

        /// <summary>
        /// Make REST PUT call and wait for the response
        /// </summary>
        public async Task<RESTResponse<T>> PutAsync<T, T1, T2>(T1 body, T2 queryString)
        {
            //TODO: This method currently remains untested. But can be tested by uncommenting this line.");
            return await CallPutAsync<T, T1, T2>(BaseUri, body, queryString, TimeoutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        public async Task<RESTResponse<T>> GetAsync<T, T1>(T1 queryString)
        {
            return await CallGetAsync<T, object, T1>(BaseUri, null, queryString, TimeoutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        /// 
        public async Task<RESTResponse<T>> GetAsync<T>()
        {
            return await CallGetAsync<T, object, object>(BaseUri, null, null, TimeoutMilliseconds, ReadToEnd);
        }

        #endregion

        #region Private Methods

        #region Get
        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        private static async Task<RESTResponse<T>> CallGetAsync<T, T1, T2>(Uri baseUri, T1 body, T2 queryString, int timeOutMilliseconds, bool readToEnd)
        {
            var retVal = await CallAsync<T, T1, T2>(baseUri, body, queryString, HttpVerb.Get, timeOutMilliseconds, readToEnd);
            return retVal;
        }

        #endregion

        #region Base Calls

        /// <summary>
        /// Make REST call and wait for the response
        /// </summary>
        private static async Task<WebResponse> CallAsync<T1, T2>(Uri baseUri, T1 body, T2 queryString, HttpVerb verb, int timeOutMilliseconds)
        {
            try
            {
                //Get the Http Request object
                var request = await GetRequestAsync(baseUri, body, queryString, verb, timeOutMilliseconds);

                //Make the call to the server and wait for the response
                var response = await request.GetResponseAsync();

                //Return the response
                return response;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The request timed out"))
                {
                    //The REST call timed out so throw this exception
                    throw new RESTTimeoutException(timeOutMilliseconds / 1000);
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static async Task<RESTResponse<T>> CallAsync<T, T1, T2>(Uri baseUri, T1 body, T2 queryString, HttpVerb verb, int timeOutMilliseconds, bool readToEnd)
        {
            var webResponse = await CallAsync(baseUri, body, queryString, verb, timeOutMilliseconds);

            var restResponse = new RESTResponse();
            restResponse.Response = webResponse;
            restResponse.Data = await GetDataFromResponseStreamAsync(webResponse, readToEnd);

            var retVal = await DeserialiseResponseAsync<T>(restResponse);

            return retVal;
        }

        #endregion

        #region Post

        /// <summary>
        /// Make Post call and wait for the response
        /// </summary>
        private static async Task<RESTResponse<T>> CallPostAsync<T, T1, T2>(Uri baseUri, T1 body, T2 queryString, int timeOutMilliseconds, bool readToEnd)
        {
            var retVal = await CallAsync<T, T1, T2>(baseUri, body, queryString, HttpVerb.Post, timeOutMilliseconds, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Put


        /// <summary>
        /// Make Post call and wait for the response
        /// </summary>
        private static async Task<RESTResponse<T>> CallPutAsync<T, T1, T2>(Uri baseUri, T1 body, T2 queryString, int timeOutMilliseconds, bool readToEnd)
        {
            //var retVal = new RESTResponse<T>();
            var retVal = await CallAsync<T, T1, T2>(baseUri, body, queryString, HttpVerb.Put, timeOutMilliseconds, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a HttpWebRequest so that the REST call can be made with it
        /// </summary>
        private static async Task<WebRequest> GetRequestAsync<T, T2>(Uri baseUri, T body, T2 queryString, HttpVerb verb, int timeOutMilliseconds)
        {
            var theUri = baseUri;

            if (queryString != null)
            {
                var queryStringText = await SerializationAdapter.SerializeAsync<T>(queryString);
                queryStringText = Uri.EscapeDataString(queryStringText);
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
            retVal.ContentType = "application/json";

            if (body != null && new List<HttpVerb> { HttpVerb.Post, HttpVerb.Put }.Contains(verb))
            {
                //Set the body of the POST/PUT

                //Serialised JSon data
                var markup = await SerializationAdapter.SerializeAsync<T>(body);

                //Get the json as a byte array
                var markupBuffer = await SerializationAdapter.DecodeStringAsync(markup);

                using (var requestStream = await retVal.GetRequestStreamAsync())
                {
                    requestStream.Write(markupBuffer, 0, markupBuffer.Length);
                }
            }

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
        private static async Task<string> GetDataFromResponseStreamAsync(WebResponse response, bool readToEnd)
        {
            var responseStream = response.GetResponseStream();
            byte[] responseBuffer = null;

            if (!readToEnd)
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
                return reader.ReadToEnd();
            }

            //Convert the response from bytes to json string 
            return await SerializationAdapter.EncodeStringAsync(responseBuffer);
        }

        /// <summary>
        /// Turn a non-generic RESTResponse in to a generic one. 
        /// </summary>
        private static async Task<RESTResponse<T>> DeserialiseResponseAsync<T>(RESTResponse response)
        {
            var retVal = new RESTResponse<T>();

            //Deserialise the json to the generic type
            retVal.Data = await SerializationAdapter.DeserializeAsync<T>(response.Data);

            //Set the HttpWebResponse
            retVal.Response = response.Response;

            return retVal;
        }

        #endregion

        #endregion
    }
}