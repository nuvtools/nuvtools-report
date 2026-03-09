namespace NuvTools.Report.Sheet.Parsing.Converters;

/// <summary>
/// Defines a converter that transforms a raw string value into a typed object.
/// </summary>
public interface IFieldConverter
{
    /// <summary>
    /// Converts the raw string value from a field into the target type.
    /// </summary>
    /// <param name="value">The raw string value from the field.</param>
    /// <param name="format">An optional format string (e.g. date format).</param>
    /// <returns>The converted value, or <c>null</c> if the value cannot be converted.</returns>
    object? Convert(string value, string? format);
}
