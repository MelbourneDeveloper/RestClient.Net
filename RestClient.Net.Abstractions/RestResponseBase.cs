using System.Threading.Tasks;

namespace RestClientDotNet.Abstractions
{
    public abstract class RestResponseBase<TBody> : RestResponseBase
    {
        protected RestResponseBase(
        IRestHeadersCollection restHeadersCollection,
        int statusCode,
        HttpVerb httpVerb,
        TBody body
        ) : base(restHeadersCollection, statusCode, httpVerb)
        {
            Body = body;
        }

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator TBody(RestResponseBase<TBody> readResult)
#pragma warning restore CA2225 // Operator overloads have named alternates
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return readResult.Body;
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        public TBody Body { get; }
    }

    public abstract class RestResponseBase
    {
        protected RestResponseBase(
        IRestHeadersCollection restHeadersCollection,
        int statusCode,
        HttpVerb httpVerb
        )
        {
            StatusCode = statusCode;
            Headers = restHeadersCollection;
            HttpVerb = httpVerb;
        }

        public int StatusCode { get; }
        public IRestHeadersCollection Headers { get; }
        public HttpVerb HttpVerb { get; }
        public abstract Task<T> ReadResponseAsync<T>();
    }
}