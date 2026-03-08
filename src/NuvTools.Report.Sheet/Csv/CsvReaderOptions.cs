using System.Text;
using NuvTools.Report.Sheet.Extensions;

namespace NuvTools.Report.Sheet.Csv;

/// <summary>
/// Options that control CSV reading behavior. Values here override attribute-level settings.
/// </summary>
public sealed class CsvReaderOptions
{
    /// <summary>
    /// Gets or sets the delimiter. When set, overrides the <see cref="Attributes.CsvRecordAttribute"/> value.
    /// </summary>
    public CsvDelimiter? Delimiter { get; set; }

    /// <summary>
    /// Gets or sets a custom delimiter string. Required when <see cref="Delimiter"/> is <see cref="CsvDelimiter.Custom"/>.
    /// </summary>
    public string? CustomDelimiter { get; set; }

    /// <summary>
    /// Gets or sets whether to skip the first row (header). Defaults to <c>false</c>.
    /// </summary>
    public bool SkipHeader { get; set; }

    /// <summary>
    /// Gets or sets whether to ignore empty lines. Defaults to <c>true</c>.
    /// </summary>
    public bool IgnoreEmptyLines { get; set; } = true;

    /// <summary>
    /// Gets or sets the encoding used to decode byte/base64 input. Defaults to <see cref="Encoding.UTF8"/>.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets whether to handle RFC 4180 quoted fields. Defaults to <c>true</c>.
    /// </summary>
    public bool HandleQuotedFields { get; set; } = true;
}
