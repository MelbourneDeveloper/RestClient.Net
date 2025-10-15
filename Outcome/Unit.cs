namespace Outcome;

/// <summary>
/// Represents a void return type or a type with only one value.
/// </summary>
/// <remarks>
/// Unit is a type that has exactly one value, similar to void in functional programming.
/// It is commonly used as a return type for operations that don't return a meaningful value,
/// or as a type parameter when a generic type is required but no data needs to be carried.
/// This implementation uses a sealed record with a private constructor to ensure only one
/// instance exists (accessible via <see cref="Value"/>), providing referential equality.
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed record Unit
{
    /// <summary>
    /// Gets the single value of the Unit type.
    /// </summary>
    /// <value>
    /// The singleton instance of Unit.
    /// </value>
    public static Unit Value { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Unit"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor is private to enforce the singleton pattern.
    /// Use <see cref="Value"/> to access the single instance.
    /// </remarks>
    private Unit() { }
}
