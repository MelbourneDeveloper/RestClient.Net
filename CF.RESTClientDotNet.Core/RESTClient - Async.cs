using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public static partial class RESTClient
    {
        #region Public Methods

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public static async Task<RESTResponse<T>> PostAsync<T>(string url, object data, int timeOutMilliseconds = 10000, bool readToEnd = true)
        {
            return await CallPostAsync<T>(url, data, null, timeOutMilliseconds, readToEnd);
        }

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public static async Task<WebResponse> PostAsync(string url, object body, int timeOutMilliseconds = 10000)
        {
            return await CallAsync(url, body, HttpVerb.Post, timeOutMilliseconds);
        }

        /// <summary>
        /// Make REST PUT call and wait for the response
        /// </summary>
        public static async Task<RESTResponse<T>> PutAsync<T>(string url, object data, int timeOutMilliseconds = 10000, bool readToEnd = true)
        {
            throw new NotImplementedException("This method currently remains untested. But can be tested by uncommenting this line.");
            return await CallPutAsync<T>(url, data, null, timeOutMilliseconds, readToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        public static async Task<RESTResponse<T>> GetAsync<T>(string url, string id, int timeOutMilliseconds = 10000, bool readToEnd = false)
        {
            return await CallGetAsync<T>(url, id, null, timeOutMilliseconds, readToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        /// 
        public static async Task<RESTResponse<T>> GetAsync<T>(string url, int timeOutMilliseconds = 10000, bool readToEnd = true)
        {
            return await CallGetAsync<T>(url, null, null, timeOutMilliseconds, readToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        public static async Task<RESTResponse> GetAsync(string url, int timeOutMilliseconds = 10000, bool readToEnd = false)
        {
            return await CallGetAsync(url, null, timeOutMilliseconds, readToEnd);
        }

        #endregion

        #region Private Methods

        #region Get
        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        private static async Task<RESTResponse<T>> CallGetAsync<T>(string url, string id, RESTResultAction<T> responseCallback, int timeOutMilliseconds, bool readToEnd = false)
        {
            //Create the return value
            var retVal = new RESTResponse<T>();

            var response = await CallGetAsync(url, id, timeOutMilliseconds, readToEnd);
            retVal = DeserialiseResponse<T>(response);

            //Return the retVal
            return retVal;
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        private static async Task<RESTResponse> CallGetAsync(string url, string id, int timeOutMilliseconds, bool readToEnd = false)
        {
            var retVal = new RESTResponse();

            retVal.Response = await CallAsync(url, id, HttpVerb.Get, timeOutMilliseconds);

            //Get the stream from the server
            retVal.Data = await GetDataFromResponseStreamAsync(retVal.Response, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Base Calls

        /// <summary>
        /// Make REST call and wait for the response
        /// </summary>
        private static async Task<WebResponse> CallAsync(string url, object body, HttpVerb verb, int timeOutMilliseconds)
        {
            try
            {
                //Get the Http Request object
                var request = await GetRequestAsync(url, body, verb, timeOutMilliseconds);

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

        /// <summary>
        /// Make REST call and wait for the response with type argument
        /// </summary>

        private static async Task<RESTResponse<T>> CallAsync<T>(string url, object body, HttpVerb verb, RESTResultAction<T> responseCallback, int timeOutMilliseconds)
        {
            //Create the return value
            var retVal = new RESTResponse<T>();

            var response = await CallPostAsync<T>(url, body.ToString(), responseCallback, timeOutMilliseconds);
            retVal = DeserialiseResponse<T>(response);

            //Return the retVal
            return retVal;
        }

        #endregion

        #region Post
        /// <summary>
        /// Make Post call and wait for the response with type argument
        /// </summary>
        private static async Task<RESTResponse<T>> CallPostAsync<T>(string url, object data, RESTResultAction<T> responseCallback, int timeOutMilliseconds, bool readToEnd = false)
        {
            var response = await CallPostAsync(url, data, timeOutMilliseconds, readToEnd);
            var retVal = DeserialiseResponse<T>(response);

            //Return the retVal
            return retVal;
        }

        /// <summary>
        /// Make Post call and wait for the response
        /// </summary>
        private static async Task<RESTResponse> CallPostAsync(string url, object data, int timeOutMilliseconds, bool readToEnd = false)
        {
            var retVal = new RESTResponse();

            retVal.Response = await CallAsync(url, data, HttpVerb.Post, timeOutMilliseconds);

            //Get the stream from the server
            retVal.Data = await GetDataFromResponseStreamAsync(retVal.Response, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Put


        /// <summary>
        /// Make Post call and wait for the response with type argument
        /// </summary>
        private static async Task<RESTResponse<T>> CallPutAsync<T>(string url, object data, RESTResultAction<T> responseCallback, int timeOutMilliseconds, bool readToEnd = false)

        {
            var response = await CallPutAsync(url, data, timeOutMilliseconds, readToEnd);
            var retVal = DeserialiseResponse<T>(response);

            //Return the retVal
            return retVal;
        }

        /// <summary>
        /// Make Post call and wait for the response
        /// </summary>
        private static async Task<RESTResponse> CallPutAsync(string url, object data, int timeOutMilliseconds, bool readToEnd = false)
        {
            var retVal = new RESTResponse();
            retVal.Response = await CallAsync(url, data, HttpVerb.Put, timeOutMilliseconds);

            //Get the stream from the server
            retVal.Data = await GetDataFromResponseStreamAsync(retVal.Response, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a HttpWebRequest so that the REST call can be made with it
        /// </summary>
        private static async Task<WebRequest> GetRequestAsync(string url, object argument, HttpVerb verb, int timeOutMilliseconds)
        {
            if (verb == HttpVerb.Get && argument != null)
            {
                url += "/" + argument.ToString();
            }

            //Create the web request
            var retVal = (HttpWebRequest)WebRequest.Create(new Uri(url));

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

            if (verb == HttpVerb.Post || verb == HttpVerb.Put)
            {
                //Set the body of the POST/PUT

                //Serialised JSon data
                var jSon = SerializationAdapter.Serialize(argument);

                //Get the json as a byte array
                var jSonBuffer = SerializationAdapter.DecodeString(jSon);

                using (var requestStream = await retVal.GetRequestStreamAsync())
                {
                    requestStream.Write(jSonBuffer, 0, jSonBuffer.Length);
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
        private static async Task<string> GetDataFromResponseStreamAsync(WebResponse response, bool readToEnd = false)
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
            return SerializationAdapter.EncodeString(responseBuffer);
        }

        /// <summary>
        /// Turn a non-generic RESTResponse in to a generic one. 
        /// </summary>
        private static RESTResponse<T> DeserialiseResponse<T>(RESTResponse response)
        {
            var retVal = new RESTResponse<T>();

            //Deserialise the json to the generic type
            retVal.Data = SerializationAdapter.Deserialize<T>(response.Data);

            //Set the HttpWebResponse
            retVal.Response = response.Response;

            return retVal;
        }

        #endregion

        #endregion
    }
}