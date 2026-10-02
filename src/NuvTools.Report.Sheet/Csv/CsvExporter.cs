using System.Text;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Sheet.Csv;

/// <summary>
/// Exports documents to CSV format directly from the Document model, without Excel intermediaries.
/// </summary>
public class CsvExporter : ICsvExporter
{
    /// <inheritdoc />
    public List<string> ExportToCsv(Document document, CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null, bool includeHeader = true, bool sanitizeDelimiter = true)
    {
        ArgumentNullException.ThrowIfNull(document);

        var delimiterString = delimiter.ToDelimiterString(customDelimiter);

        var result = new List<string>();

        foreach (var table in document.Tables)
            result.Add(ConvertToBase64String(BuildCsvLines(table, delimiterString, includeHeader, sanitizeDelimiter)));

        return result;
    }

    /// <inheritdoc />
    public string ExportFirstSheetToCsv(Document document, CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null, bool includeHeader = true, bool sanitizeDelimiter = true)
    {
        ArgumentNullException.ThrowIfNull(document);

        var delimiterString = delimiter.ToDelimiterString(customDelimiter);

        var table = document.Tables.First();

        return ConvertToBase64String(BuildCsvLines(table, delimiterString, includeHeader, sanitizeDelimiter));
    }

    /// <summary>
    /// Builds CSV lines from a table by iterating rows and cells directly.
    /// </summary>
    private static List<string> BuildCsvLines(Table.Models.Table table, string delimiter, bool includeHeader, bool sanitizeDelimiter)
    {
        var lines = new List<string>();

        // Header row
        if (includeHeader && table.Content.Header?.Columns is { } columns)
        {
            var headerLine = string.Join(delimiter,
                columns.OrderBy(c => c.Order).Select(c => Sanitize(c.Label, delimiter, sanitizeDelimiter)));
            lines.Add(headerLine);
        }

        // Data rows
        foreach (var row in table.Content.Rows)
        {
            var cells = row.Cells.OrderBy(c => c.Column.Order).ToList();
            var line = string.Join(delimiter, cells.Select(c => Sanitize(FormatValue(c), delimiter, sanitizeDelimiter)));
            lines.Add(line);
        }

        return lines;
    }

    /// <summary>
    /// Applies the column format to date values, leaving every other value untouched.
    /// </summary>
    private static string? FormatValue(Cell cell)
    {
        if (!string.IsNullOrEmpty(cell.Column.Format) && DateTimeOffset.TryParse(cell.Value, out var date))
            return date.ToString(cell.Column.Format);

        return cell.Value;
    }

    /// <summary>
    /// Removes occurrences of the delimiter from a value to prevent CSV corruption.
    /// </summary>
    private static string? Sanitize(string? value, string delimiter, bool sanitize)
    {
        if (!sanitize || string.IsNullOrEmpty(value))
            return value;

        return value.Replace(delimiter, string.Empty);
    }

    /// <summary>
    /// Converts a list of CSV lines to a base64-encoded string.
    /// </summary>
    /// <remarks>
    /// The content is UTF-8 with a byte order mark, which Excel requires to detect the encoding.
    /// </remarks>
    private static string ConvertToBase64String(List<string> lines)
    {
        using MemoryStream ms = new();
        var sw = new StreamWriter(ms, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        lines.ForEach(a => sw.WriteLine(a));
        sw.Flush();

        return Convert.ToBase64String(ms.ToArray());
    }
}
