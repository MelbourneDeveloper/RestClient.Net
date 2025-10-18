using System.Text.RegularExpressions;

namespace RestClient.Net.OpenApiGenerator;

/// <summary>Helper methods for code generation.</summary>
internal static partial class CodeGenerationHelpers
{
    /// <summary>Converts a string to PascalCase.</summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The PascalCase version of the text.</returns>
    public static string ToPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var parts = text.Split(['-', '_', ' '], StringSplitOptions.RemoveEmptyEntries);
        return string.Join(string.Empty, parts.Select(p => char.ToUpperInvariant(p[0]) + p[1..]));
    }

    /// <summary>Converts a string to camelCase.</summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The camelCase version of the text.</returns>
    public static string ToCamelCase(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var pascal = ToPascalCase(text);
        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }

    /// <summary>Indents text by the specified number of levels.</summary>
    /// <param name="text">The text to indent.</param>
    /// <param name="level">The indentation level (1 level = 4 spaces).</param>
    /// <returns>The indented text.</returns>
    public static string Indent(string text, int level)
    {
        var indent = new string(' ', level * 4);
        return string.Join('\n', text.Split('\n').Select(line => indent + line));
    }

    /// <summary>Builds a path expression from a path template.</summary>
    /// <param name="path">The path template.</param>
    /// <returns>The path expression.</returns>
    public static string BuildPathExpression(string path) => path;

    /// <summary>Regular expression for matching path parameters.</summary>
    [GeneratedRegex(@"\{[^}]+\}")]
    public static partial Regex PathParameterRegex();
}
