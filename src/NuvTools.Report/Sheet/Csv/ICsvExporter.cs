using NuvTools.Report.Table.Models;

namespace NuvTools.Report.Sheet.Csv;

/// <summary>
/// Exports documents to CSV format encoded as base64 strings.
/// </summary>
public interface ICsvExporter
{
    /// <summary>
    /// Exports all tables in a document to separate CSV files encoded as base64 strings.
    /// </summary>
    /// <param name="document">The document containing tables to export.</param>
    /// <param name="delimiter">The delimiter to use. Defaults to <see cref="CsvDelimiter.Comma"/>.</param>
    /// <param name="customDelimiter">A custom delimiter string, required when <paramref name="delimiter"/> is <see cref="CsvDelimiter.Custom"/>.</param>
    /// <param name="includeHeader">Whether to include the header row with column labels. Defaults to <c>true</c>.</param>
    /// <param name="sanitizeDelimiter">Whether to remove occurrences of the delimiter from header labels and cell values to prevent CSV corruption. Defaults to <c>true</c>.</param>
    /// <returns>A list of base64-encoded CSV strings, one for each table in the document.</returns>
    List<string> ExportToCsv(Document document, CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null, bool includeHeader = true, bool sanitizeDelimiter = true);

    /// <summary>
    /// Exports the first table in a document to a CSV file encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing the table to export.</param>
    /// <param name="delimiter">The delimiter to use. Defaults to <see cref="CsvDelimiter.Comma"/>.</param>
    /// <param name="customDelimiter">A custom delimiter string, required when <paramref name="delimiter"/> is <see cref="CsvDelimiter.Custom"/>.</param>
    /// <param name="includeHeader">Whether to include the header row with column labels. Defaults to <c>true</c>.</param>
    /// <param name="sanitizeDelimiter">Whether to remove occurrences of the delimiter from header labels and cell values to prevent CSV corruption. Defaults to <c>true</c>.</param>
    /// <returns>A base64-encoded CSV string of the first table.</returns>
    string ExportFirstSheetToCsv(Document document, CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null, bool includeHeader = true, bool sanitizeDelimiter = true);
}
