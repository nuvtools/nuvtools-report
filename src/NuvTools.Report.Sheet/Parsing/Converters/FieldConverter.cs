namespace NuvTools.Report.Sheet.Parsing.Converters;

/// <summary>
/// Generic abstract base class for type-safe field converters.
/// </summary>
/// <typeparam name="T">The target type of the conversion.</typeparam>
public abstract class FieldConverter<T> : IFieldConverter
{
    /// <summary>
    /// Converts the raw string value to the target type.
    /// </summary>
    /// <param name="value">The raw string value from the field.</param>
    /// <param name="format">An optional format string.</param>
    /// <returns>The converted value, or <c>null</c> if the value cannot be converted.</returns>
    public abstract T? Convert(string value, string? format);

    object? IFieldConverter.Convert(string value, string? format) => Convert(value, format);
}
