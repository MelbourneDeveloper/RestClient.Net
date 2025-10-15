using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Exhaustion;

/// <summary>
/// Pure functions for detecting discard patterns in switch expressions and statements.
/// </summary>
internal static class DiscardDetection
{
    /// <summary>
    /// Checks if a pattern is a top-level discard (catch-all) pattern.
    /// This does not check nested subpatterns, only the outermost pattern.
    /// </summary>
    /// <param name="pattern">The pattern to check.</param>
    /// <returns>True if the pattern is a top-level discard, false otherwise.</returns>
    public static bool IsTopLevelDiscard(PatternSyntax pattern)
    {
        // Direct discard pattern
        if (pattern is DiscardPatternSyntax)
        {
            return true;
        }

        // Var pattern with discard designation
        if (
            pattern is VarPatternSyntax varPattern
            && varPattern.Designation is DiscardDesignationSyntax
        )
        {
            return true;
        }

        // Declaration pattern with 'object' type is a catch-all
        if (
            pattern is DeclarationPatternSyntax declPattern
            && declPattern.Type.ToString() == "object"
        )
        {
            return true;
        }

        // Do NOT check subpatterns - we only care about top-level discards
        return false;
    }
}
