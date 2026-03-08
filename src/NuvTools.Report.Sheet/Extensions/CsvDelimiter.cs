namespace NuvTools.Report.Sheet.Extensions;

/// <summary>
/// Specifies the delimiter character used when exporting CSV files.
/// </summary>
public enum CsvDelimiter
{
    /// <summary>Comma (<c>,</c>) — the standard CSV delimiter per RFC 4180.</summary>
    Comma,
    /// <summary>Semicolon (<c>;</c>).</summary>
    Semicolon,
    /// <summary>Tab character (<c>\t</c>).</summary>
    Tab,
    /// <summary>Colon (<c>:</c>).</summary>
    Colon,
    /// <summary>Space (<c> </c>).</summary>
    Space,
    /// <summary>A custom delimiter string provided via the <c>customDelimiter</c> parameter.</summary>
    Custom
}

/// <summary>
/// Extension methods for <see cref="CsvDelimiter"/>.
/// </summary>
public static class CsvDelimiterExtensions
{
    /// <summary>
    /// Converts a <see cref="CsvDelimiter"/> value to its corresponding string.
    /// </summary>
    public static string ToDelimiterString(this CsvDelimiter delimiter, string? customDelimiter = null) => delimiter switch
    {
        CsvDelimiter.Comma => ",",
        CsvDelimiter.Semicolon => ";",
        CsvDelimiter.Tab => "\t",
        CsvDelimiter.Colon => ":",
        CsvDelimiter.Space => " ",
        CsvDelimiter.Custom => string.IsNullOrEmpty(customDelimiter)
            ? throw new ArgumentNullException(nameof(customDelimiter),
                "A custom delimiter string must be provided when using CsvDelimiter.Custom.")
            : customDelimiter,
        _ => throw new ArgumentOutOfRangeException(nameof(delimiter))
    };
}
