using NuvTools.Report.Parsing;
using NuvTools.Report.Parsing.Converters;

namespace NuvTools.Report.Sheet.Csv.Attributes;

/// <summary>
/// Maps a property to a CSV field by position and provides optional conversion settings.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CsvFieldAttribute : Attribute
{
    /// <summary>
    /// Gets the zero-based position of the field in the CSV record.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Gets or sets a display name for the field.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Gets or sets the type of an <see cref="IFieldConverter"/> implementation used to convert the raw string value.
    /// </summary>
    public Type? Converter { get; set; }

    /// <summary>
    /// Gets or sets a format string passed to the converter (e.g. <c>"yyyyMMdd"</c> for dates).
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the trim mode applied to the raw field value before conversion. Defaults to <see cref="TrimMode.None"/>.
    /// </summary>
    public TrimMode Trim { get; set; } = TrimMode.None;

    /// <summary>
    /// Initializes a new instance of <see cref="CsvFieldAttribute"/> with the specified zero-based field position.
    /// </summary>
    /// <param name="order">The zero-based position of the field in the CSV record.</param>
    public CsvFieldAttribute(int order)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(order);
        Order = order;
    }
}
