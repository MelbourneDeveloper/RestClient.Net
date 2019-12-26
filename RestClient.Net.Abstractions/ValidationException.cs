using System;

namespace RestClientDotNet.Abstractions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
}
