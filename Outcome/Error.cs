namespace Outcome;

// This is the workaround for the lack of Discriminated Unions in C#
#pragma warning disable CA1034 // Nested types should not be visible

public abstract partial record Result<TSuccess, TFailure>
{
    /// <summary>
    /// Represents a failed computation containing an error of type TFailure.
    /// This is the "sad path" case where something went wrong.
    ///
    /// In functional programming, this represents the Left side of an Either type,
    /// allowing for explicit error handling without exceptions.
    /// </summary>
    /// <param name="Value">The error/failure value.</param>
#pragma warning disable CA1716 // Identifiers should not match keywords - TODO: Consider renaming to Failure?
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
    public sealed record Error<TSuccess, TFailure>(TFailure Value) : Result<TSuccess, TFailure>
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>
        /// Converts a failure value to an Error result.
        /// This enables clean error creation without verbose constructors.
        /// </summary>
        /// <param name="error">The error to wrap in an Error result.</param>
        /// <returns>An Error result containing the error.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static Error<TSuccess, TFailure> FromTFailure(TFailure error) => new(error);
#pragma warning restore CA1000 // Do not declare static members on generic types

        /// <summary>
        /// Returns a string representation of the failed result.
        /// </summary>
        /// <returns>A string representation of the error.</returns>
        public override string ToString() => $"Error({Value})";
    }
}
