namespace AutomationFramework.Core.Extensions;

/// <summary>
/// General-purpose string helpers used throughout the framework.
/// </summary>
public static class StringExtensions
{
    /// <summary>Returns true when the string is null, empty, or whitespace.</summary>
    public static bool IsNullOrWhiteSpace(this string? value) =>
        string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Truncates the string to <paramref name="maxLength"/> characters,
    /// appending "…" when truncation occurs.
    /// </summary>
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;
        return string.Concat(value.AsSpan(0, maxLength - 1), "…");
    }

    /// <summary>
    /// Converts a PascalCase or camelCase string to snake_case.
    /// Useful for mapping C# property names to JSON keys.
    /// </summary>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return System.Text.RegularExpressions.Regex
            .Replace(value, "([a-z0-9])([A-Z])", "$1_$2")
            .ToLowerInvariant();
    }

    /// <summary>Masks all but the last four characters with '*'.</summary>
    public static string MaskSensitive(this string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= 4)
            return new string('*', value?.Length ?? 0);
        return new string('*', value.Length - 4) + value[^4..];
    }
}
