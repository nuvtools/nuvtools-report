using System.Text;

namespace NuvTools.Report.Sheet.FixedLength;

/// <summary>
/// Options that control fixed-length record reading behavior.
/// </summary>
public sealed class FixedLengthReaderOptions
{
    /// <summary>
    /// Gets or sets whether to ignore empty lines. Defaults to <c>true</c>.
    /// </summary>
    public bool IgnoreEmptyLines { get; set; } = true;

    /// <summary>
    /// Gets or sets the encoding used to decode byte/base64 input. Defaults to <see cref="Encoding.UTF8"/>.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets an optional filter predicate applied to each raw line before parsing.
    /// Return <c>true</c> to include the line, <c>false</c> to skip it.
    /// </summary>
    public Func<string, bool>? LineFilter { get; set; }
}
