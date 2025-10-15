namespace Outcome;

// This is the workaround for the lack of Discriminated Unions in C#
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1000 // Do not declare static members on generic types

public abstract partial record Result<TSuccess, TFailure>
{
    /// <summary>
    /// Represents a successful computation containing a value of type TSuccess.
    /// This is the "happy path" case where everything worked as expected.
    ///
    /// In functional programming, this represents the Right side of an Either type
    /// or the Some case of an Option type with additional context.
    /// </summary>
    /// <param name="Value">The successful result value.</param>
#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
    public sealed record Ok<TSuccess, TFailure>(TSuccess Value) : Result<TSuccess, TFailure>
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
    {
        /// <summary>
        /// Returns a string representation of the successful result.
        /// </summary>
        /// <returns>A string representation of the success.</returns>
        public override string ToString() => $"Ok({Value})";
    }
}
