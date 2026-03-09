namespace NuvTools.Report.Sheet.Csv.Attributes;

/// <summary>
/// Specifies CSV record-level settings such as the delimiter character.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CsvRecordAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the delimiter used to separate fields. Defaults to <see cref="CsvDelimiter.Comma"/>.
    /// </summary>
    public CsvDelimiter Delimiter { get; set; } = CsvDelimiter.Comma;

    /// <summary>
    /// Gets or sets a custom delimiter string. Required when <see cref="Delimiter"/> is <see cref="CsvDelimiter.Custom"/>.
    /// </summary>
    public string? CustomDelimiter { get; set; }
}
