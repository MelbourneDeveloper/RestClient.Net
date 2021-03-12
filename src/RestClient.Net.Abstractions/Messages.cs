using System;

namespace RestClient.Net.Abstractions
{
    internal static class Messages
    {
        public const string InfoSendReturnedNoException = "SendAsync on HttpClient returned without an exception";
        public const string ErrorMessageAbsoluteUriAsString = "Absolute Uris cannot be specified as a string. Change the argument to a Uri and change the UriKind to relative.";
        public const string ErrorMessageDeserialization = "An error occurred while attempting to deserialize the response";
        public const string ErrorMessageHeaderAlreadyExists = "The Content-Type header has already been set";
        public static string GetErrorMessageNonSuccess(int responseCode, Uri? requestUri) => $"Non successful Http Status Code: {responseCode}.\r\nRequest Uri: {requestUri}";

    }
}
