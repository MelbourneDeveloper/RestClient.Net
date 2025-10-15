namespace Outcome;

/// <summary>
/// Extension methods that provide additional functional programming utilities for Result.
/// These methods enable more fluent and expressive Result-based code.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Transforms a sequence of Results into a Result of a sequence.
    /// If all Results are Ok, returns Ok with all the values.
    /// If any Result is Error, returns the first Error encountered.
    ///
    /// This is the sequence/traverse operation for Result, enabling
    /// all-or-nothing semantics for collections of Results.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="results">The sequence of Results to traverse.</param>
    /// <returns>A Result containing all success values or the first error.</returns>
    public static Result<IReadOnlyList<TSuccess>, TFailure> Sequence<TSuccess, TFailure>(
        this IEnumerable<Result<TSuccess, TFailure>> results
    )
    {
        var successList = new List<TSuccess>();

        foreach (var result in results)
        {
            if (result is Result<TSuccess, TFailure>.Error<TSuccess, TFailure> error)
            {
                return Result<IReadOnlyList<TSuccess>, TFailure>.Failure(error.Value);
            }

            if (result is Result<TSuccess, TFailure>.Ok<TSuccess, TFailure> ok)
            {
                successList.Add(ok.Value);
            }
        }

        return new Result<IReadOnlyList<TSuccess>, TFailure>.Ok<IReadOnlyList<TSuccess>, TFailure>(
            successList.AsReadOnly()
        );
    }

    /// <summary>
    /// Flattens a nested Result structure.
    /// This is useful when you have Result&lt;Result&lt;T, E&gt;, E&gt; and want Result&lt;T, E&gt;.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result">The nested Result to flatten.</param>
    /// <returns>A flattened Result.</returns>
    public static Result<TSuccess, TFailure> Flatten<TSuccess, TFailure>(
        this Result<Result<TSuccess, TFailure>, TFailure> result
    ) => result.Bind(innerResult => innerResult);

    /// <summary>
    /// Combines two Results using a combining function.
    /// Both Results must be Ok for the combination to succeed.
    /// </summary>
    /// <typeparam name="TSuccess1">The success type of the first Result.</typeparam>
    /// <typeparam name="TSuccess2">The success type of the second Result.</typeparam>
    /// <typeparam name="TSuccess">The success type of the combined Result.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result1">The first Result.</param>
    /// <param name="result2">The second Result.</param>
    /// <param name="combiner">Function to combine the two success values.</param>
    /// <returns>Ok with combined value if both are Ok, otherwise the first Error.</returns>
    public static Result<TSuccess, TFailure> Combine<TSuccess1, TSuccess2, TSuccess, TFailure>(
        this Result<TSuccess1, TFailure> result1,
        Result<TSuccess2, TFailure> result2,
        Func<TSuccess1, TSuccess2, TSuccess> combiner
    ) => result1.Bind(value1 => result2.Map(value2 => combiner(value1, value2)));

    /// <summary>
    /// Filters a Result based on a predicate.
    /// If the Result is Ok and the predicate returns true, returns the original Result.
    /// If the Result is Ok but the predicate returns false, returns Error with the provided error.
    /// If the Result is already Error, returns it unchanged.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result">The Result to filter.</param>
    /// <param name="predicate">The predicate to test the success value.</param>
    /// <param name="errorOnFalse">The error to return if the predicate fails.</param>
    /// <returns>The original Result if predicate passes, Error if predicate fails.</returns>
    public static Result<TSuccess, TFailure> Filter<TSuccess, TFailure>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, bool> predicate,
        TFailure errorOnFalse
    ) =>
        result.Bind(value =>
            predicate(value)
                ? new Result<TSuccess, TFailure>.Ok<TSuccess, TFailure>(value)
                : Result<TSuccess, TFailure>.Failure(errorOnFalse)
        );

    /// <summary>
    /// ⚠️ Dangerous Method - Use with Caution! ⚠️
    /// Extracts the success value from a Result, or throws an InvalidOperationException if the Result is an Error.
    /// This is useful in test scenarios where you expect a success and want to fail the test if an error occurs.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result">The Result to extract the value from.</param>
    /// <param name="errorMessage">Optional custom error message to use when throwing. Defaults to "Expected success result".</param>
    /// <returns>The success value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Result is an Error.</exception>
    public static TSuccess GetValueOrThrow<TSuccess, TFailure>(
        this Result<TSuccess, TFailure> result,
        string errorMessage = "Expected success result"
    ) => result.Match(s => s, _ => throw new InvalidOperationException(errorMessage));

    /// <summary>
    /// ⚠️ Dangerous Method - Use with Caution! ⚠️
    /// Extracts the error value from a Result, or throws an InvalidOperationException if the Result is a success.
    /// This is useful in test scenarios where you expect an error and want to fail the test if a success occurs.
    /// </summary>
    /// <typeparam name="TSuccess">The success type.</typeparam>
    /// <typeparam name="TFailure">The failure type.</typeparam>
    /// <param name="result">The Result to extract the error from.</param>
    /// <param name="errorMessage">Optional custom error message to use when throwing. Defaults to "Expected error result".</param>
    /// <returns>The failure value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Result is a success.</exception>
    public static TFailure GetErrorOrThrow<TSuccess, TFailure>(
        this Result<TSuccess, TFailure> result,
        string errorMessage = "Expected error result"
    ) => result.Match(_ => throw new InvalidOperationException(errorMessage), e => e);
}
