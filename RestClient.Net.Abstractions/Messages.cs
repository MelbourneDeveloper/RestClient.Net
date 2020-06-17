namespace RestClient.Net.Abstractions
{
    public static class Messages
    {
        public const string ErrorMessageAbsoluteUriAsString = "Absolute Uris cannot be specified as a string. Change the argument to a Uri and change the UriKind to relative.";
        public const string ErrorMessageDeserialization = "An error occurred while attempting to deserialize the response";
        public const string ErrorMessageHeaderAlreadyExists = "The Content-Type header has already been set";
    }
}
