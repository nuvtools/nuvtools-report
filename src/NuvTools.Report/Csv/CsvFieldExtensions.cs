using System.Reflection;
using NuvTools.Report.Csv.Attributes;

namespace NuvTools.Report.Csv;

/// <summary>
/// Utility methods for extracting field metadata from types decorated with <see cref="CsvFieldAttribute"/>.
/// </summary>
public static class CsvFieldExtensions
{
    /// <summary>
    /// Returns the <see cref="CsvFieldAttribute.Caption"/> values for all decorated properties, ordered by <see cref="CsvFieldAttribute.Order"/>.
    /// </summary>
    public static IEnumerable<string> GetFieldCaptions(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new { Property = p, Attribute = p.GetCustomAttribute<CsvFieldAttribute>() })
            .Where(x => x.Attribute is not null)
            .OrderBy(x => x.Attribute!.Order)
            .Select(x => x.Attribute!.Caption ?? x.Property.Name);
    }

    /// <summary>
    /// Generates a CSV header string from the <see cref="CsvFieldAttribute.Caption"/> values.
    /// </summary>
    public static string GetCsvHeader(this Type type, CsvDelimiter delimiter = CsvDelimiter.Semicolon, string? customDelimiter = null)
    {
        return string.Join(delimiter.ToDelimiterString(customDelimiter), type.GetFieldCaptions());
    }
}
