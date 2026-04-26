using System.Collections.Concurrent;
using System.Reflection;
using NuvTools.Report.Sheet.Csv.Attributes;
using NuvTools.Report.Parsing;
using NuvTools.Report.Parsing.Converters;

namespace NuvTools.Report.Sheet.Csv;

/// <summary>
/// Reads CSV content and maps it to strongly-typed objects using attribute-based field mapping.
/// </summary>
public class CsvReader : ICsvReader
{
    private static readonly ConcurrentDictionary<Type, FieldMapping[]> MetadataCache = new();

    /// <summary>
    /// Reads CSV from a string and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadString<T>(string content, CsvReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(content);
        return ParseLines<T>(content, options ?? new CsvReaderOptions());
    }

    /// <summary>
    /// Reads CSV from a base64-encoded string and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadBase64<T>(string base64Content, CsvReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(base64Content);
        var opts = options ?? new CsvReaderOptions();
        var bytes = Convert.FromBase64String(base64Content);
        var content = opts.Encoding.GetString(bytes);
        return ParseLines<T>(content, opts);
    }

    /// <summary>
    /// Reads CSV from a byte array and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadBytes<T>(byte[] data, CsvReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(data);
        var opts = options ?? new CsvReaderOptions();
        var content = opts.Encoding.GetString(data);
        return ParseLines<T>(content, opts);
    }

    /// <summary>
    /// Reads CSV from a stream and returns a list of mapped objects.
    /// </summary>
    public List<T> ReadStream<T>(Stream stream, CsvReaderOptions? options = null) where T : new()
    {
        ArgumentNullException.ThrowIfNull(stream);
        var opts = options ?? new CsvReaderOptions();
        using var reader = new StreamReader(stream, opts.Encoding, leaveOpen: true);
        var content = reader.ReadToEnd();
        return ParseLines<T>(content, opts);
    }

    private static List<T> ParseLines<T>(string content, CsvReaderOptions options) where T : new()
    {
        var mappings = MetadataCache.GetOrAdd(typeof(T), BuildMappings);
        var delimiter = ResolveDelimiter<T>(options);
        var lines = SplitLines(content);

        var startIndex = options.SkipHeader ? 1 : 0;
        var results = new List<T>();

        for (var i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];

            if (options.IgnoreEmptyLines && string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var fields = options.HandleQuotedFields
                    ? SplitQuotedFields(line, delimiter)
                    : line.Split(delimiter);

                var record = new T();
                PopulateRecord(record, fields, mappings, i + 1, line);
                results.Add(record);
            }
            catch (ParseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ParseException(
                    $"Error parsing CSV line {i + 1}: {ex.Message}",
                    i + 1, rawLine: line, innerException: ex);
            }
        }

        return results;
    }

    private static string ResolveDelimiter<T>(CsvReaderOptions options)
    {
        if (options.Delimiter.HasValue)
            return options.Delimiter.Value.ToDelimiterString(options.CustomDelimiter);

        var attr = typeof(T).GetCustomAttribute<CsvRecordAttribute>();
        if (attr is not null)
            return attr.Delimiter.ToDelimiterString(attr.CustomDelimiter);

        return CsvDelimiter.Comma.ToDelimiterString();
    }

    private static FieldMapping[] BuildMappings(Type type)
    {
        var mappings = new List<FieldMapping>();

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = prop.GetCustomAttribute<CsvFieldAttribute>();
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
                attr.Format,
                attr.Trim,
                converter));
        }

        if (mappings.Count == 0)
            throw new InvalidOperationException(
                $"Type '{type}' has no properties decorated with [CsvField]. At least one is required.");

        return [.. mappings.OrderBy(m => m.Order)];
    }

    private static void PopulateRecord<T>(T record, string[] fields, FieldMapping[] mappings, int lineNumber, string rawLine)
    {
        foreach (var mapping in mappings)
        {
            var fieldIndex = mapping.Order;

            if (fieldIndex >= fields.Length)
            {
                // Field not present in this line — skip if property is nullable, otherwise error
                if (IsNullableProperty(mapping.Property))
                    continue;

                throw new ParseException(
                    $"Line {lineNumber}: expected at least {mapping.Order + 1} fields but found {fields.Length}.",
                    lineNumber, mapping.Order, rawLine);
            }

            var rawValue = fields[fieldIndex];
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
                    value = BuiltInConverters.Convert(rawValue, mapping.Property.PropertyType, mapping.Format);
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
                    lineNumber, mapping.Order, rawLine, ex);
            }
        }
    }

    private static bool IsNullableProperty(PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) is not null)
            return true;

        if (!property.PropertyType.IsValueType)
        {
            var context = new NullabilityInfoContext();
            var info = context.Create(property);
            return info.WriteState == NullabilityState.Nullable;
        }

        return false;
    }

    private static string ApplyTrim(string value, TrimMode trim) => trim switch
    {
        TrimMode.Left => value.TrimStart(),
        TrimMode.Right => value.TrimEnd(),
        TrimMode.Both => value.Trim(),
        _ => value
    };

    private static string[] SplitLines(string content)
    {
        return content.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
    }

    /// <summary>
    /// Splits a CSV line into fields, handling RFC 4180 quoted fields.
    /// Does not support multiline quoted fields.
    /// </summary>
    private static string[] SplitQuotedFields(string line, string delimiter)
    {
        var fields = new List<string>();
        var i = 0;
        var delimLen = delimiter.Length;

        while (i <= line.Length)
        {
            if (i == line.Length)
            {
                fields.Add(string.Empty);
                break;
            }

            if (line[i] == '"')
            {
                // Quoted field
                var sb = new System.Text.StringBuilder();
                i++; // skip opening quote

                while (i < line.Length)
                {
                    if (line[i] == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            // Escaped quote
                            sb.Append('"');
                            i += 2;
                        }
                        else
                        {
                            // Closing quote
                            i++;
                            break;
                        }
                    }
                    else
                    {
                        sb.Append(line[i]);
                        i++;
                    }
                }

                fields.Add(sb.ToString());

                // Skip delimiter after closing quote
                if (i < line.Length && line.AsSpan(i).StartsWith(delimiter))
                    i += delimLen;
            }
            else
            {
                // Unquoted field
                var nextDelim = line.IndexOf(delimiter, i, StringComparison.Ordinal);
                if (nextDelim == -1)
                {
                    fields.Add(line[i..]);
                    break;
                }
                else
                {
                    fields.Add(line[i..nextDelim]);
                    i = nextDelim + delimLen;
                }
            }
        }

        return [.. fields];
    }

    private sealed record FieldMapping(
        PropertyInfo Property,
        int Order,
        string? Format,
        TrimMode Trim,
        IFieldConverter? Converter);
}
