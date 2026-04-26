namespace NuvTools.Report.Parsing;

/// <summary>
/// Exception thrown when a record line or field cannot be parsed.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="ParseException"/>.
/// </remarks>
public sealed class ParseException(string message, int lineNumber, int? fieldIndex = null, string? rawLine = null, Exception? innerException = null) : Exception(message, innerException)
{
    /// <summary>
    /// Gets the 1-based line number where the error occurred.
    /// </summary>
    public int LineNumber { get; } = lineNumber;

    /// <summary>
    /// Gets the 1-based field index where the error occurred, if applicable.
    /// </summary>
    public int? FieldIndex { get; } = fieldIndex;

    /// <summary>
    /// Gets the raw line content that caused the error.
    /// </summary>
    public string? RawLine { get; } = rawLine;
}
