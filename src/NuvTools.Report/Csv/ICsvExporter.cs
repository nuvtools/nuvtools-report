using NuvTools.Report.Table.Models;

namespace NuvTools.Report.Csv;

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
    /// <returns>A list of base64-encoded CSV strings, one for each table in the document.</returns>
    List<string> ExportToCsv(Document document, CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null);

    /// <summary>
    /// Exports the first table in a document to a CSV file encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing the table to export.</param>
    /// <param name="delimiter">The delimiter to use. Defaults to <see cref="CsvDelimiter.Comma"/>.</param>
    /// <param name="customDelimiter">A custom delimiter string, required when <paramref name="delimiter"/> is <see cref="CsvDelimiter.Custom"/>.</param>
    /// <returns>A base64-encoded CSV string of the first table.</returns>
    string ExportFirstSheetToCsv(Document document, CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null);
}
