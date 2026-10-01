namespace Exhaustion;

/// <summary>
/// Signals that a switch cannot be analyzed within its bounded complexity budget.
/// </summary>
#pragma warning disable CA1064 // This control-flow exception is caught within the analyzer.
internal sealed class AnalysisLimitExceededException : Exception { }
#pragma warning restore CA1064
