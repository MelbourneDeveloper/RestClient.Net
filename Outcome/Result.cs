namespace Outcome;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

/// <summary>
/// A discriminated union representing either a successful result or a failure.
/// This type enforces explicit error handling and eliminates the need for exceptions
/// in business logic, following the Railway Oriented Programming pattern.
///
/// The Result type is a closed hierarchy - only Ok and Error instances can exist,
/// ensuring complete pattern matching coverage and type safety.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TFailure">The type of the failure value.</typeparam>
public abstract partial record Result<TSuccess, TFailure>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TSuccess, TFailure}"/> class.
    /// Private constructor ensures this is a closed hierarchy.
    /// Only Ok and Error can inherit from Result.
    /// </summary>
    private Result() { }

    /// <summary>
    /// Gets a value indicating whether this result represents a successful computation.
    /// In functional programming, this is analogous to checking if an Either is Right.
    /// </summary>
    public bool IsOk => this is Ok<TSuccess, TFailure>;

    /// <summary>
    /// Gets a value indicating whether this result represents a failed computation.
    /// In functional programming, this is analogous to checking if an Either is Left.
    /// </summary>
    public bool IsError => this is Error<TSuccess, TFailure>;

    /// <summary>
    /// Creates a failed result containing the specified error.
    /// This is the canonical way to create an error in the Result monad.
    /// </summary>
    /// <param name="error">The error value.</param>
    /// <returns>An Error result containing the error.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static Result<TSuccess, TFailure> Failure(TFailure error) =>
        new Error<TSuccess, TFailure>(error);
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <summary>
    /// Transforms the success value of this result using the provided function.
    /// If this is an Error, the error is propagated unchanged.
    ///
    /// This is the fundamental Functor operation for Result, enabling
    /// composition of operations while preserving the error context.
    /// </summary>
    /// <typeparam name="TNewSuccess">The type of the transformed success value.</typeparam>
    /// <param name="mapper">The function to apply to the success value.</param>
    /// <returns>A new result with the transformed success value or the original error.</returns>
    public Result<TNewSuccess, TFailure> Map<TNewSuccess>(Func<TSuccess, TNewSuccess> mapper) =>
        this switch
        {
            Ok<TSuccess, TFailure>(var value) => new Result<TNewSuccess, TFailure>.Ok<
                TNewSuccess,
                TFailure
            >(mapper(value)),
            Error<TSuccess, TFailure>(var error) => Result<TNewSuccess, TFailure>.Failure(error),
        };

    /// <summary>
    /// Transforms the error value of this result using the provided function.
    /// If this is an Ok, the success value is propagated unchanged.
    ///
    /// This enables error transformation and recovery scenarios while
    /// preserving the success path.
    /// </summary>
    /// <typeparam name="TNewFailure">The type of the transformed error value.</typeparam>
    /// <param name="mapper">The function to apply to the error value.</param>
    /// <returns>A new result with the original success value or the transformed error.</returns>
    public Result<TSuccess, TNewFailure> MapError<TNewFailure>(
        Func<TFailure, TNewFailure> mapper
    ) =>
        this switch
        {
            Ok<TSuccess, TFailure>(var value) => new Result<TSuccess, TNewFailure>.Ok<
                TSuccess,
                TNewFailure
            >(value),
            Error<TSuccess, TFailure>(var error) => Result<TSuccess, TNewFailure>.Failure(
                mapper(error)
            ),
        };

    /// <summary>
    /// Monadic bind operation for Result. Applies a function that returns a Result
    /// to the success value of this Result, flattening the nested Result.
    ///
    /// This is the core operation that makes Result a monad, enabling
    /// Railway Oriented Programming and safe composition of operations.
    /// </summary>
    /// <typeparam name="TNewSuccess">The success type of the result returned by the binder.</typeparam>
    /// <param name="binder">The function to apply if this is a success.</param>
    /// <returns>The result of applying the binder, or the original error.</returns>
    public Result<TNewSuccess, TFailure> Bind<TNewSuccess>(
        Func<TSuccess, Result<TNewSuccess, TFailure>> binder
    ) =>
        this switch
        {
            Ok<TSuccess, TFailure>(var value) => binder(value),
            Error<TSuccess, TFailure>(var error) => Result<TNewSuccess, TFailure>.Failure(error),
        };

    /// <summary>
    /// Applies one of two functions based on the state of this Result.
    /// This is the catamorphism for Result, allowing for complete pattern matching.
    ///
    /// Use this when you need to extract a value of the same type from both
    /// success and error cases.
    /// </summary>
    /// <typeparam name="TResult">The type of value to return.</typeparam>
    /// <param name="onSuccess">Function to apply if this is a success.</param>
    /// <param name="onError">Function to apply if this is an error.</param>
    /// <returns>The result of applying the appropriate function.</returns>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onError
    ) =>
        this switch
        {
            Ok<TSuccess, TFailure>(var value) => onSuccess(value),
            Error<TSuccess, TFailure>(var error) => onError(error),
        };

    /// <summary>
    /// Performs a side effect based on the state of this Result without changing the Result.
    /// This enables logging, debugging, or other side effects while preserving
    /// the functional nature of the Result chain.
    /// </summary>
    /// <param name="onSuccess">Action to perform if this is a success.</param>
    /// <param name="onError">Action to perform if this is an error.</param>
    /// <returns>This Result unchanged.</returns>
    public Result<TSuccess, TFailure> Tap(
        Action<TSuccess>? onSuccess = null,
        Action<TFailure>? onError = null
    )
    {
#pragma warning disable IDE0010 // Add missing cases
        switch (this)
        {
            case Ok<TSuccess, TFailure>(var value):
                onSuccess?.Invoke(value);
                break;
            case Error<TSuccess, TFailure>(var error):
                onError?.Invoke(error);
                break;
        }
#pragma warning restore IDE0010 // Add missing cases

        return this;
    }

    /// <summary>
    /// Returns the success value if this is Ok, otherwise returns the provided default value.
    /// This is a safe way to extract values from Results with a fallback.
    /// </summary>
    /// <param name="defaultValue">The value to return if this is an Error.</param>
    /// <returns>The success value or the default value.</returns>
    public TSuccess GetValueOrDefault(TSuccess defaultValue) =>
        this is Ok<TSuccess, TFailure>(var value) ? value : defaultValue;

    /// <summary>
    /// Returns the success value if this is Ok, otherwise returns the result of calling the provider function.
    /// This enables lazy evaluation of default values.
    /// </summary>
    /// <param name="defaultProvider">Function to call if this is an Error.</param>
    /// <returns>The success value or the result of the default provider.</returns>
    public TSuccess GetValueOrDefault(Func<TSuccess> defaultProvider) =>
        this is Ok<TSuccess, TFailure>(var value) ? value : defaultProvider();

    /// <summary>
    /// Unary negation operator to extract the error value if this is an Error,
    /// otherwise returns null. This provides a convenient way to extract errors for checking.
    /// </summary>
    /// <param name="result">The result to extract error from.</param>
    /// <returns>The error value if Error, null if Ok.</returns>
#pragma warning disable CA2225 // Provide a method named 'LogicalNot' as a friendly alternate for operator op_LogicalNot
    public static TFailure operator !(Result<TSuccess, TFailure> result)
#pragma warning restore CA2225 // Provide a method named 'LogicalNot' as a friendly alternate for operator op_LogicalNot
    {
        return result is Error<TSuccess, TFailure>(var error)
            ? error
            : throw new InvalidOperationException("Expected error result");
    }

    /// <summary>
    /// ⚠️ Danger. Returns the value or throws if Error. Only for use in tests.
    /// </summary>
    /// <param name="result">The result to extract the success value from.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Result is an Error.</exception>
#pragma warning disable CA2225 // Provide a method named 'Plus' as a friendly alternate for operator op_UnaryPlus
    public static TSuccess operator +(Result<TSuccess, TFailure> result)
#pragma warning restore CA2225 // Provide a method named 'Plus' as a friendly alternate for operator op_UnaryPlus
    {
        return result is Ok<TSuccess, TFailure>(var value)
            ? value
            : throw new InvalidOperationException("Expected success result");
    }
}
