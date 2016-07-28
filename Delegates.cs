namespace CF.RESTClientDotNet
{
    public delegate void RESTResultAction<T>(RESTResponse<T> responseCallback);
    public delegate void RESTResultAction(RESTResponse responseCallback);
}
