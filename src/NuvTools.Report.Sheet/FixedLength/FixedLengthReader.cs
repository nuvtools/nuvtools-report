using System.Collections.Concurrent;
using System.Reflection;
using NuvTools.Report.Sheet.FixedLength.Attributes;
using NuvTools.Report.Sheet.Parsing;
using NuvTools.Report.Sheet.Parsing.Converters;

namespace NuvTools.Report.Sheet.FixedLength;

/// <summary>
/// Reads fixed-length (positional) content and maps it to strongly-typed objects using attribute-based field mapping.
/// </summary>
public class FixedLengthReader : IFixedLengthReader
{
    private static readonly ConcurrentDictionary<Type, FieldMapping[]> MetadataCache = new();

    /// <summary>
    /// Reads fixed-length records from a string and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadString<T>(string content, FixedLengthReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(content);
        return ParseLines<T>(content, options ?? new FixedLengthReaderOptions());
    }

    /// <summary>
    /// Reads fixed-length records from a base64-encoded string and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadBase64<T>(string base64, FixedLengthReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(base64);
        var opts = options ?? new FixedLengthReaderOptions();
        var bytes = Convert.FromBase64String(base64);
        var content = opts.Encoding.GetString(bytes);
        return ParseLines<T>(content, opts);
    }

    /// <summary>
    /// Reads fixed-length records from a byte array and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadBytes<T>(byte[] data, FixedLengthReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(data);
        var opts = options ?? new FixedLengthReaderOptions();
        var content = opts.Encoding.GetString(data);
        return ParseLines<T>(content, opts);
    }

    /// <summary>
    /// Reads fixed-length records from a stream and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadStream<T>(Stream stream, FixedLengthReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(stream);
        var opts = options ?? new FixedLengthReaderOptions();
        using var reader = new StreamReader(stream, opts.Encoding, leaveOpen: true);
        var content = reader.ReadToEnd();
        return ParseLines<T>(content, opts);
    }

    private static List<T> ParseLines<T>(string content, FixedLengthReaderOptions options) where T : new()
    {
        var mappings = MetadataCache.GetOrAdd(typeof(T), BuildMappings);
        var allowShorterLines = typeof(T).GetCustomAttribute<FixedLengthRecordAttribute>()?.AllowShorterLines ?? false;
        var lines = content.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);

        var results = new List<T>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (options.IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                continue;

            if (options.LineFilter is not null && !options.LineFilter(line))
                continue;

            try
            {
                var record = new T();
                PopulateRecord(record, line, mappings, i + 1, allowShorterLines);
                results.Add(record);
            }
            catch (ParseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ParseException(
                    $"Error parsing fixed-length line {i + 1}: {ex.Message}",
                    i + 1, rawLine: line, innerException: ex);
            }
        }

        return results;
    }

    private static FieldMapping[] BuildMappings(Type type)
    {
        var mappings = new List<FieldMapping>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = prop.GetCustomAttribute<FixedLengthFieldAttribute>();
            if (attr is null) continue;

            IFieldConverter? converter = null;
            if (attr.Converter is not null)
            {
                if (!typeof(IFieldConverter).IsAssignableFrom(attr.Converter))
                    throw new InvalidOperationException(
                        $"Converter type '{attr.Converter}' on property '{prop.Name}' does not implement IFieldConverter. Consider using FieldConverter<T> as a base class.");

                converter = (IFieldConverter)Activator.CreateInstance(attr.Converter)!;
            }

            mappings.Add(new FieldMapping(
                prop,
                attr.Order,
                attr.Length,
                attr.Optional,
                attr.Format,
                attr.Trim,
                converter));
        }

        if (mappings.Count == 0)
            throw new InvalidOperationException(
                $"Type '{type}' has no properties decorated with [FixedLengthField]. At least one is required.");

        // Order by Order, then calculate start offsets
        var ordered = mappings.OrderBy(m => m.Order).ToArray();
        var offset = 0;
        for (var i = 0; i < ordered.Length; i++)
        {
            ordered[i] = ordered[i] with { StartOffset = offset };
            offset += ordered[i].Length;
        }

        return ordered;
    }

    private static void PopulateRecord<T>(T record, string line, FieldMapping[] mappings, int lineNumber, bool allowShorterLines)
    {
        foreach (var mapping in mappings)
        {
            // Check if field starts beyond the line
            if (mapping.StartOffset >= line.Length)
            {
                if (allowShorterLines || mapping.Optional)
                    continue;

                throw new ParseException(
                    $"Line {lineNumber}: expected at least {mapping.StartOffset + mapping.Length} characters but found {line.Length}.",
                    lineNumber, mapping.Order, line);
            }

            // Extract the field value — may be shorter than expected length if line is short
            var availableLength = Math.Min(mapping.Length, line.Length - mapping.StartOffset);

            if (availableLength < mapping.Length && !allowShorterLines && !mapping.Optional)
            {
                throw new ParseException(
                    $"Line {lineNumber}: field at position {mapping.Order} expected {mapping.Length} characters but only {availableLength} available.",
                    lineNumber, mapping.Order, line);
            }

            var rawValue = line.Substring(mapping.StartOffset, availableLength);
            rawValue = ApplyTrim(rawValue, mapping.Trim);

            try
            {
                object? value;

                if (mapping.Converter is not null)
                {
                    value = mapping.Converter.Convert(rawValue, mapping.Format);
                }
                else
                {
                    // For non-nullable value types, whitespace-only fields return default
                    var targetType = mapping.Property.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(targetType);
                    var isNullable = underlyingType is not null || !targetType.IsValueType;

                    if (string.IsNullOrWhiteSpace(rawValue) && !isNullable && targetType != typeof(string))
                    {
                        value = targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                    }
                    else
                    {
                        value = BuiltInConverters.Convert(rawValue, targetType, mapping.Format);
                    }
                }

                mapping.Property.SetValue(record, value);
            }
            catch (ParseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ParseException(
                    $"Line {lineNumber}, field {mapping.Order} ('{mapping.Property.Name}'): {ex.Message}",
                    lineNumber, mapping.Order, line, ex);
            }
        }
    }

    private static string ApplyTrim(string value, TrimMode trim) => trim switch
    {
        TrimMode.Left => value.TrimStart(),
        TrimMode.Right => value.TrimEnd(),
        TrimMode.Both => value.Trim(),
        _ => value
    };

    private sealed record FieldMapping(
        PropertyInfo Property,
        int Order,
        int Length,
        bool Optional,
        string? Format,
        TrimMode Trim,
        IFieldConverter? Converter)
    {
        public int StartOffset { get; init; }
    }
}
