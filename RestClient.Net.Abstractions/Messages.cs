namespace RestClientDotNet.Abstractions
{
    public static class Messages
    {
        public const string ErrorMessageAbsoluteUriAsString = "Absolute Uris cannot be specified as a string. Change the argument to a Uri and change the UriKind to relative.";
        public const string ErrorMessageDeserialization = "An error occurred while attempting to deserialze the response";
    }
}
