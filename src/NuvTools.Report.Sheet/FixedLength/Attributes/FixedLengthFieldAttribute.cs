using NuvTools.Report.Sheet.Parsing;
using NuvTools.Report.Sheet.Parsing.Converters;

namespace NuvTools.Report.Sheet.FixedLength.Attributes;

/// <summary>
/// Maps a property to a fixed-length field by sequential position and character width.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class FixedLengthFieldAttribute : Attribute
{
    /// <summary>
    /// Gets the zero-based sequential position of the field in the record.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Gets the character width of the field.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets or sets whether the field is optional (skipped if the line is too short). Defaults to <c>false</c>.
    /// </summary>
    public bool Optional { get; set; }

    /// <summary>
    /// Gets or sets a format string passed to the converter (e.g. <c>"yyyyMMdd"</c> for dates, or precision for decimals).
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Gets or sets the trim mode applied to the raw field value before conversion. Defaults to <see cref="TrimMode.None"/>.
    /// </summary>
    public TrimMode Trim { get; set; } = TrimMode.None;

    /// <summary>
    /// Gets or sets the type of an <see cref="IFieldConverter"/> implementation used to convert the raw string value.
    /// </summary>
    public Type? Converter { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="FixedLengthFieldAttribute"/> with the specified order and character width.
    /// </summary>
    /// <param name="order">The zero-based sequential position of the field.</param>
    /// <param name="length">The character width of the field.</param>
    public FixedLengthFieldAttribute(int order, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(order);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        Order = order;
        Length = length;
    }
}
