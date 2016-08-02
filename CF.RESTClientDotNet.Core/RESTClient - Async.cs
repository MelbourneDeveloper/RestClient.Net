using System;
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
        public async Task<RESTResponse<T>> PostAsync<T, T1>(Uri baseUri, T1 body)
        {
            return await CallPostAsync<T, T1>(BaseUri, body, TimeoutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public async Task<WebResponse> PostAsync<T, T1>(T1 body)
        {
            return await CallAsync(BaseUri, body, HttpVerb.Post, TimeoutMilliseconds);
        }

        /// <summary>
        /// Make REST PUT call and wait for the response
        /// </summary>
        public async Task<RESTResponse<T>> PutAsync<T, T1>(T1 body)
        {
            //TODO: This method currently remains untested. But can be tested by uncommenting this line.");
            return await CallPutAsync<T, T1>(BaseUri, body, null, TimeoutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        public async Task<RESTResponse<T>> GetAsync<T, T1>(T1 body)
        {
            return await CallGetAsync<T, T1>(BaseUri, body, TimeoutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        /// 
        public async Task<RESTResponse<T>> GetAsync<T, T1>()
        {
            return await CallGetAsync<T, T1>(BaseUri, default(T1), TimeoutMilliseconds, ReadToEnd);
        }

        #endregion

        #region Private Methods

        #region Get
        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        private static async Task<RESTResponse<T>> CallGetAsync<T, T1>(Uri baseUri, T1 body, int timeOutMilliseconds, bool readToEnd)
        {
            var retVal = await CallAsync<T, T1>(baseUri, body, HttpVerb.Get, timeOutMilliseconds, readToEnd);
            return retVal;
        }

        #endregion

        #region Base Calls

        /// <summary>
        /// Make REST call and wait for the response
        /// </summary>
        private static async Task<WebResponse> CallAsync<T1>(Uri baseUri, T1 body, HttpVerb verb, int timeOutMilliseconds)
        {
            try
            {
                //Get the Http Request object
                var request = await GetRequestAsync(baseUri, body, verb, timeOutMilliseconds);

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

        private static async Task<RESTResponse<T>> CallAsync<T, T1>(Uri baseUri, T1 body, HttpVerb verb, int timeOutMilliseconds, bool readToEnd)
        {
            var webResponse = await CallAsync(baseUri, body, verb, timeOutMilliseconds);

            var restResponse = new  RESTResponse();
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
        private static async Task<RESTResponse<T>> CallPostAsync<T, T1>(Uri baseUri, T1 body, int timeOutMilliseconds, bool readToEnd)
        {
            var retVal = await CallAsync<T, T1>(baseUri, body, HttpVerb.Post, timeOutMilliseconds, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Put


        /// <summary>
        /// Make Post call and wait for the response with type argument
        /// </summary>
        private static async Task<RESTResponse<T>> CallPutAsync<T, T1>(Uri baseUri, T1 body, RESTResultAction<T> responseCallback, int timeOutMilliseconds, bool readToEnd)

        {
            var retVal = await CallAsync<T, T1>(baseUri, body, HttpVerb.Put, timeOutMilliseconds, readToEnd);
            return retVal;
        }

        /// <summary>
        /// Make Post call and wait for the response
        /// </summary>
        private static async Task<RESTResponse<T>> CallPutAsync<T, T1>(Uri baseUri, T1 body, int timeOutMilliseconds, bool readToEnd)
        {
            //var retVal = new RESTResponse<T>();
            var retVal = await CallAsync<T, T1>(baseUri, body, HttpVerb.Put, timeOutMilliseconds, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a HttpWebRequest so that the REST call can be made with it
        /// </summary>
        private static async Task<WebRequest> GetRequestAsync<T>(Uri baseUri, T body, HttpVerb verb, int timeOutMilliseconds)
        {
            //Create the web request
            var retVal = (HttpWebRequest)WebRequest.Create(baseUri);

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
                var jSon = await SerializationAdapter.SerializeAsync<T>(body);


                //jSon = Uri.EscapeDataString(jSon);

                //Get the json as a byte array
                var jSonBuffer = await SerializationAdapter.DecodeStringAsync(jSon);

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
        private static async Task<string> GetDataFromResponseStreamAsync(WebResponse response, bool readToEnd )
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