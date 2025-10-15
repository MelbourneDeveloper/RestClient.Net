namespace Outcome;

// This is the workaround for the lack of Discriminated Unions in C#
#pragma warning disable CA1034 // Nested types should not be visible
// Not relevant here
#pragma warning disable CA1000 // Do not declare static members on generic types

public abstract partial record HttpError<TError>
{
    /// <summary>
    /// Represents an error caused by an exception during the HTTP operation.
    /// This typically occurs due to network issues, timeouts, or other infrastructure problems.
    ///
    /// Examples: Network unreachable, connection timeout, DNS resolution failure.
    /// </summary>
    /// <param name="Exception">The exception that caused the error.</param>
    public sealed record ExceptionError(Exception Exception) : HttpError<TError>
    {
        /// <summary>
        /// Converts an Exception to an ExceptionError.
        /// Enables clean error creation from caught exceptions.
        /// </summary>
        /// <param name="exception">The exception to wrap.</param>
        /// <returns>An ExceptionError containing the exception.</returns>
        public static new ExceptionError FromException(Exception exception) => new(exception);

        /// <summary>
        /// Returns a string representation of the exception error.
        /// </summary>
        /// <returns>A string representation of the exception error.</returns>
        public override string ToString() =>
            $"ExceptionError({Exception.GetType().Name}: {Exception.Message})";
    }
}
