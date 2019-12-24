using System;

namespace RestClientDotNet.Abstractions
{
#pragma warning disable CA1012 // Abstract types should not have constructors
    public abstract class RestClientBase : IDisposable
#pragma warning restore CA1012 // Abstract types should not have constructors
    {
        #region Fields
        private bool disposed;
        #endregion

        #region Public Properties
        public abstract IRestHeadersCollection DefaultRequestHeaders { get; }
        public bool ThrowExceptionOnFailure { get; set; } = true;
        public string DefaultContentType { get; set; } = "application/json";
        public ISerializationAdapter SerializationAdapter { get; }
        public abstract TimeSpan Timeout { get; set; }
        public ITracer Tracer { get; }
        #endregion

        #region Constructor
        public RestClientBase(ISerializationAdapter serializationAdapter, ITracer tracer)
        {
            SerializationAdapter = serializationAdapter;
            Tracer = tracer;
        }
        #endregion


        #region Public Methods
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
