using System.Globalization;

namespace NuvTools.Report.Parsing.Converters;

/// <summary>
/// Provides default type conversion logic for field values.
/// </summary>
internal static class BuiltInConverters
{
    /// <summary>
    /// Converts a raw string to the specified target type using built-in conversion rules.
    /// </summary>
    internal static object? Convert(string value, Type targetType, string? format)
    {
        var (underlyingType, isNullable) = UnwrapNullable(targetType);

        if (isNullable && string.IsNullOrWhiteSpace(value))
            return null;

        if (underlyingType == typeof(string))
            return value;

        if (underlyingType == typeof(int))
            return int.Parse(value, CultureInfo.InvariantCulture);

        if (underlyingType == typeof(long))
            return long.Parse(value, CultureInfo.InvariantCulture);

        if (underlyingType == typeof(short))
            return short.Parse(value, CultureInfo.InvariantCulture);

        if (underlyingType == typeof(decimal))
            return decimal.Parse(value, CultureInfo.InvariantCulture);

        if (underlyingType == typeof(double))
            return double.Parse(value, CultureInfo.InvariantCulture);

        if (underlyingType == typeof(float))
            return float.Parse(value, CultureInfo.InvariantCulture);

        if (underlyingType == typeof(bool))
            return ConvertBool(value);

        if (underlyingType == typeof(DateTime))
            return ConvertDateTime(value, format);

        if (underlyingType == typeof(DateOnly))
            return ConvertDateOnly(value, format);

        if (underlyingType == typeof(TimeOnly))
            return ConvertTimeOnly(value, format);

        if (underlyingType == typeof(Guid))
            return Guid.Parse(value);

        if (underlyingType.IsEnum)
            return Enum.Parse(underlyingType, value, ignoreCase: true);

        throw new NotSupportedException($"Type '{targetType}' is not supported for field conversion. Use a custom IFieldConverter.");
    }

    private static (Type underlyingType, bool isNullable) UnwrapNullable(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        return underlying is not null ? (underlying, true) : (type, false);
    }

    private static bool ConvertBool(string value) => value switch
    {
        "1" => true,
        "0" => false,
        _ => bool.Parse(value)
    };

    private static DateTime ConvertDateTime(string value, string? format) =>
        string.IsNullOrEmpty(format)
            ? DateTime.Parse(value, CultureInfo.InvariantCulture)
            : DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);

    private static DateOnly ConvertDateOnly(string value, string? format) =>
        string.IsNullOrEmpty(format)
            ? DateOnly.Parse(value, CultureInfo.InvariantCulture)
            : DateOnly.ParseExact(value, format, CultureInfo.InvariantCulture);

    private static TimeOnly ConvertTimeOnly(string value, string? format) =>
        string.IsNullOrEmpty(format)
            ? TimeOnly.Parse(value, CultureInfo.InvariantCulture)
            : TimeOnly.ParseExact(value, format, CultureInfo.InvariantCulture);
}
