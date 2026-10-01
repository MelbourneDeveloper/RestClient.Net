namespace Exhaustion;

/// <summary>
/// Bounds the amount of work and materialized coverage for one switch analysis.
/// </summary>
/// <param name="cancellationToken">The compiler's cancellation token.</param>
internal sealed class AnalysisBudget(CancellationToken cancellationToken = default)
{
    /// <summary>
    /// Maximum number of required constructor combinations and leaf names.
    /// </summary>
    internal const int MaximumCombinations = 1024;

    /// <summary>
    /// Maximum number of symbol and pattern traversal steps.
    /// </summary>
    internal const int MaximumWork = 16384;

    /// <summary>
    /// Maximum hierarchy or constructor expansion nesting depth.
    /// </summary>
    internal const int MaximumDepth = 64;

    private int work;
    private int combinations;

    /// <summary>
    /// Gets the compiler's cancellation token.
    /// </summary>
    internal CancellationToken CancellationToken { get; } = cancellationToken;

    /// <summary>
    /// Gets the remaining number of combinations that may be materialized.
    /// </summary>
    internal int RemainingCombinations => MaximumCombinations - combinations;

    /// <summary>
    /// Checks cancellation and reserves one traversal step.
    /// </summary>
    internal void Visit()
    {
        CancellationToken.ThrowIfCancellationRequested();
        if (++work > MaximumWork)
        {
            throw new AnalysisLimitExceededException();
        }
    }

    /// <summary>
    /// Checks cancellation and bounds recursive traversal depth.
    /// </summary>
    /// <param name="depth">The current recursive traversal depth.</param>
    internal void CheckDepth(int depth)
    {
        CancellationToken.ThrowIfCancellationRequested();
        if (depth > MaximumDepth)
        {
            throw new AnalysisLimitExceededException();
        }
    }

    /// <summary>
    /// Checks cancellation and reserves coverage before materializing it.
    /// </summary>
    /// <param name="count">The number of combinations or leaf names to reserve.</param>
    internal void ReserveCombinations(int count)
    {
        CancellationToken.ThrowIfCancellationRequested();
        if (count > RemainingCombinations)
        {
            throw new AnalysisLimitExceededException();
        }

        combinations += count;
    }
}
