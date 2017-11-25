using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CF.RESTClientDotNet
{
    public class RESTClient
    {
        #region Public Properties 
        public Type ErrorType { get; set; }
        public int TimeoutMilliseconds { get; set; } = 10000;
        public bool ReadToEnd { get; set; } = true;
        public static ISerializationAdapter SerializationAdapter { get; set; }
        public Uri BaseUri { get; private set; }

        /// <summary>
        /// Gets or sets the header value of Content-Type in the http request. Note: This will default to 'application/json'
        /// </summary>
        public string ContentType { get; set; } = "application/json";

#if (!SILVERLIGHT)
        /// <summary>
        /// Allows headers to be sent as part of the request. Note: it seems that this not supported in Silverlight
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

#endif

        #endregion

        #region Events
        public event EventHandler<TraceEventArgs> OperationOccurred;
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

        public async Task<TReturn> PostAsync<TReturn, TBody>(TBody body, string queryString)
        {
            return await CallAsync<TReturn, TBody, object>(body, queryString, HttpVerb.Post);
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
                OperationOccurred?.Invoke(this, new TraceEventArgs(nameof(SerializationAdapter.DecodeStringAsync), TraceEventArgs.OperationState.Start));
                //If the body is a string, convert it to binary
                var bodyAsString = body as string;
                if (bodyAsString != null)
                {
                    body = await SerializationAdapter.DecodeStringAsync(bodyAsString);
                }
                OperationOccurred?.Invoke(this, new TraceEventArgs(nameof(SerializationAdapter.DecodeStringAsync), TraceEventArgs.OperationState.Complete));

                //Get the Http Request object
                OperationOccurred?.Invoke(this, new TraceEventArgs(nameof(GetRequestAsync), TraceEventArgs.OperationState.Start));
                var request = await GetRequestAsync(body, queryString, verb);
                OperationOccurred?.Invoke(this, new TraceEventArgs(nameof(GetRequestAsync), TraceEventArgs.OperationState.Complete));

                //Get the response from the server
                OperationOccurred?.Invoke(this, new TraceEventArgs(nameof(request.GetResponseAsync), TraceEventArgs.OperationState.Start));
                var retVal = await request.GetResponseAsync();
                OperationOccurred?.Invoke(this, new TraceEventArgs(nameof(request.GetResponseAsync), TraceEventArgs.OperationState.Complete));
                return retVal;
            }
            catch (WebException wex)
            {
                var errorMessage = "The REST call returned an error. Please see Error property for details";
                object error = null;
                byte[] responseData = null;

                //TODO: This stream reader doesn't look like it does anything...
                if (wex.Response != null)
                {
                    using (var streamReader = new StreamReader(wex.Response.GetResponseStream()))
                    {
                        responseData = await GetDataFromResponseStreamAsync(wex.Response);
                        if (ErrorType != null)
                        {
                            error = await SerializationAdapter.DeserializeAsync(responseData, ErrorType);
                        }
                    }
                }

                throw new RESTException(error, responseData, errorMessage, wex);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The request timed out"))
                {
                    //The REST call timed out so throw this exception
                    throw new RESTTimeoutException(TimeoutMilliseconds / 1000);
                }
                throw;
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

            var data = await GetDataFromResponseStreamAsync(webResponse);
            var restResponse = new RESTResponse
            {
                Response = webResponse,
                Data = data
            };
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
                    var queryStringBinary = await SerializationAdapter.SerializeAsync(queryString);
                    queryStringText = await SerializationAdapter.EncodeStringAsync(queryStringBinary);
                    queryStringText = Uri.EscapeDataString(queryStringText);
                }

                //TODO: This is nasty as hell.
                //Issue #10
                var forwardSlashPart = string.Empty;
                var absoluteUriString = theUri.AbsoluteUri.ToString();
                if (!(absoluteUriString.Substring(absoluteUriString.Length - 1) == "/"))
                {
                    forwardSlashPart = "/";
                }

                theUri = new Uri(theUri.AbsoluteUri + forwardSlashPart + queryStringText);
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

            if (body != null && new List<HttpVerb> { HttpVerb.Post, HttpVerb.Put }.Contains(verb))
            {
                //Set the body of the POST/PUT
                byte[] binary;
                var bodyAsBinary = body as byte[];
                if (bodyAsBinary != null)
                {
                    //The body is already a string
                    binary = bodyAsBinary;
                }
                else
                {
                    //Serialise the data
                    binary = await SerializationAdapter.SerializeAsync(body);
                }

                using (var requestStream = await retVal.GetRequestStreamAsync())
                {
                    requestStream.Write(binary, 0, binary.Length);
                }
            }

#if (!SILVERLIGHT)
            retVal.ContentType = ContentType;

            foreach (var key in Headers?.Keys)
            {
                retVal.Headers[key] = Headers[key];
            }

            retVal.ContinueTimeout = TimeoutMilliseconds;
#endif

            //Return the request
            return retVal;
        }

        /// <summary>
        /// Given the response from the REST call, return the string(
        /// </summary>
        private async Task<byte[]> GetDataFromResponseStreamAsync(WebResponse response)
        {
            var responseStream = response.GetResponseStream();
            byte[] responseBuffer;

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
                List<byte> bytes = new List<byte>();

                //TODO: This method of getting the data looks like a performance hit because of the casting.
                var eof = false;
                while (!eof)
                {
                    var theByte = responseStream.ReadByte();

                    if (theByte == -1)
                    {
                        eof = true;
                    }
                    else
                    {
                        bytes.Add((byte)theByte);
                    }
                }

                responseBuffer = bytes.ToArray();
            }

            //Convert the response from bytes to json string 
            return responseBuffer;
        }

        /// <summary>
        /// Turn a non-generic RESTResponse in to a generic one. 
        /// </summary>
        private async Task<TReturn> DeserialiseResponseAsync<TReturn>(RESTResponse response)
        {
            TReturn retVal;

            if (typeof(TReturn) == typeof(string))
            {
                var textAsObject = (object)await SerializationAdapter.EncodeStringAsync(response.Data);
                retVal = (TReturn)textAsObject;
            }
            else
            {
                //Deserialise the json to the generic type
                retVal = await SerializationAdapter.DeserializeAsync<TReturn>(response.Data);
            }

            //Set the HttpWebResponse
            LastRestResponse = response.Response;

            return retVal;
        }

        #endregion

        #endregion
    }
}