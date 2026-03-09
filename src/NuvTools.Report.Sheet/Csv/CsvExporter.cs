using NuvTools.Report.Table.Models;

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
            var line = string.Join(delimiter, cells.Select(c => Sanitize(c.Value, delimiter, sanitizeDelimiter)));
            lines.Add(line);
        }

        return lines;
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
    private static string ConvertToBase64String(List<string> lines)
    {
        using MemoryStream ms = new();
        var sw = new StreamWriter(ms);

        lines.ForEach(a => sw.WriteLine(a));
        sw.Flush();

        return Convert.ToBase64String(ms.ToArray());
    }
}
