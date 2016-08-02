using System;
using System.IO;
using System.Net;

namespace CF.RESTClientDotNet
{
    public partial class RESTClient
    {
        #region Public Methods

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public RESTResponse<T> Post<T>(string url, object data)
        {
            return CallPost<T>(url, data, null, TimeOutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make REST POST call and wait for the response
        /// </summary>
        public WebResponse Post(string url, object body)
        {
            return Call(url, body, HttpVerb.Post, TimeOutMilliseconds);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        public RESTResponse<T> Get<T>(string url, string id)
        {
            return CallGet<T>(url, id, null, TimeOutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        /// 

        public RESTResponse<T> Get<T>(string url)
        {
            return CallGet<T>(url, null, null, TimeOutMilliseconds, ReadToEnd);
        }


        /// <summary>
        /// Make a GET call and wait for the response with a result type of T
        /// </summary>
        public void GetAsync<T>(string url, RESTResultAction<T> responseCallback)
        {
            CallGet<T>(url, null, responseCallback, TimeOutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response with a result type of T
        /// </summary>
        public void GetAsync<T>(string url, string id, RESTResultAction<T> responseCallback)
        {
            CallGet<T>(url, id, responseCallback, TimeOutMilliseconds, ReadToEnd);
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>

        public RESTResponse Get(string url)
        {
            return CallGet(url, null, TimeOutMilliseconds, ReadToEnd);
        }

        #endregion

        #region Private Methods

        #region Get
        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        private static RESTResponse<T> CallGet<T>(string url, string id, RESTResultAction<T> responseCallback, int timeOutMilliseconds, bool readToEnd)
        {
            //Create the return value
            var retVal = new RESTResponse<T>();

            if (responseCallback != null)
            {
                CallAsync(url, id, responseCallback, HttpVerb.Get, timeOutMilliseconds, readToEnd);
            }
            else
            {
                //Get the response from the server
                var response = CallGet(url, id, timeOutMilliseconds, readToEnd);
                retVal = DeserialiseResponse<T>(response);
            }

            //Return the retVal
            return retVal;
        }

        /// <summary>
        /// Make a GET call and wait for the response
        /// </summary>
        private static RESTResponse CallGet(string url, string id, int timeOutMilliseconds, bool readToEnd)
        {
            var retVal = new RESTResponse();

            retVal.Response = Call(url, id, HttpVerb.Get, timeOutMilliseconds);

            //Get the stream from the server
            retVal.Data = GetDataFromResponseStream(retVal.Response, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Base Calls

        /// <summary>
        /// Make REST POST call asynchronously
        /// </summary>
        private static void CallAsync(string url, object body, RESTResultAction responseCallback, HttpVerb verb, int timeOutMilliseconds, bool readToEnd)
        {
            //Get the Http Request object
            var request = GetRequest(url, body, verb, timeOutMilliseconds);

            //Make the call to the server and wait for the response
            request.BeginGetResponse((ar) =>
            {
                //The call has returned from server so invoke the callback
                var asyncState = (HttpWebRequest)ar.AsyncState;
                using (var response = (HttpWebResponse)asyncState.EndGetResponse(ar))
                {
                    string data = null;

                    if (verb != HttpVerb.Post)
                    {
                        data = GetDataFromResponseStream(response, readToEnd);
                    }

                    //Return control to the callback method passed in
                    responseCallback(new RESTResponse { Response = response, Data = data });
                }

            }, request);
        }

        /// <summary>
        /// Make REST POST call asynchronously with a generic return value
        /// </summary>
        private static void CallAsync<T>(string url, object body, RESTResultAction<T> responseCallback, HttpVerb verb, int timeOutMilliseconds, bool readToEnd)
        {
            //Get the Http Request object
            var request = GetRequest(url, body, verb, timeOutMilliseconds);

            //Make the call to the server and wait for the response
            request.BeginGetResponse((ar) =>
            {
                //The call has returned from server so invoke the callback
                var request2 = (HttpWebRequest)ar.AsyncState;
                using (var response = (HttpWebResponse)request2.EndGetResponse(ar))
                {
                    T data = default(T);
                    Exception error = null;
                    try
                    {
                        data = SerializationAdapter.Deserialize<T>(GetDataFromResponseStream(response, readToEnd));
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }

                    //Return control to the callback method passed in
                    responseCallback(new RESTResponse<T> { Response = response, Data = data, Error = error });
                }

            }, request);
        }

        /// <summary>
        /// Make REST call and wait for the response
        /// </summary>
        private static WebResponse Call(string url, object body, HttpVerb verb, int timeOutMilliseconds)
        {
            try
            {
                //Get the Http Request object
                var request = GetRequest(url, body, verb, timeOutMilliseconds);

                //Make the call to the server and wait for the response
                var response = request.GetResponse();

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

        #endregion

        #region Post
        /// <summary>
        /// Make Post call and wait for the response with type argument
        /// </summary>
        private static RESTResponse<T> CallPost<T>(string url, object data, RESTResultAction<T> responseCallback, int timeOutMilliseconds, bool readToEnd)
        {
            //Create the return value
            var retVal = new RESTResponse<T>();

            if (responseCallback != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                //Get the response from the server
                var response = CallPost(url, data, timeOutMilliseconds, readToEnd);
                retVal = DeserialiseResponse<T>(response);
            }

            //Return the retVal
            return retVal;
        }

        /// <summary>
        /// Make Post call and wait for the response
        /// </summary>
        private static RESTResponse CallPost(string url, object data, int timeOutMilliseconds, bool readToEnd)
        {
            var retVal = new RESTResponse();

            retVal.Response = Call(url, data, HttpVerb.Post, timeOutMilliseconds);

            //Get the stream from the server
            retVal.Data = GetDataFromResponseStream(retVal.Response, readToEnd);

            //Return the json
            return retVal;
        }

        #endregion

        #region Put

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a HttpWebRequest so that the REST call can be made with it
        /// </summary>
        private static HttpWebRequest GetRequest(string url, object argument, HttpVerb verb, int timeOutMilliseconds)
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

                //The length of the buffer (postvars) is used as contentlength.
                retVal.ContentLength = jSonBuffer.Length;

                //We open a stream for writing the postvars
                using (var requestStream = retVal.GetRequestStream())
                {
                    requestStream.Write(jSonBuffer, 0, jSonBuffer.Length);
                }
            }

            retVal.Timeout = timeOutMilliseconds;

            //Return the request
            return retVal;
        }

        /// <summary>
        /// Given the response from the REST call, return the string(
        /// </summary>
        private static string GetDataFromResponseStream(WebResponse response, bool readToEnd)
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
                var responseLength = responseStream.Read(responseBuffer, 0, (int)responseStream.Length);
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