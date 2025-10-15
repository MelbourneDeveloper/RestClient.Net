namespace Exhaustion;

/// <summary>
/// Pure functions for building diagnostic messages.
/// </summary>
internal static class DiagnosticMessages
{
    /// <summary>
    /// Builds a detailed message showing which types were matched and which are missing.
    /// </summary>
    /// <param name="baseTypeName">The name of the base type being switched on.</param>
    /// <param name="matchedTypes">Types that were matched in the switch expression.</param>
    /// <param name="missingTypes">Types that are missing from the switch expression.</param>
    /// <returns>A formatted diagnostic message.</returns>
    public static (string mainMessage, string detailMessage) BuildDetailMessage(
        string baseTypeName,
        HashSet<string> matchedTypes,
        HashSet<string> missingTypes
    )
    {
        var mainMessage =
            missingTypes.Count == 0
                ? $"Switch on {baseTypeName} has redundant default arm"
                : $"Switch on {baseTypeName} is not exhaustive";

        var parts = new List<string>();

        if (matchedTypes.Count > 0)
        {
            parts.Add($"Matched: {string.Join(", ", matchedTypes.OrderBy(x => x))}");
        }

        if (missingTypes.Count > 0)
        {
            parts.Add($"Missing: {string.Join(", ", missingTypes.OrderBy(x => x))}");
        }

        return (mainMessage, string.Join("; ", parts));
    }
}
