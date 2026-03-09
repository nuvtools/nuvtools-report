using NuvTools.Report.Table.Models;

namespace NuvTools.Report.Sheet.Excel;

/// <summary>
/// Exports documents to Excel (.xlsx) format encoded as base64 strings.
/// </summary>
public interface IExcelExporter
{
    /// <summary>
    /// Exports a document to Excel (.xlsx) format encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing tables to export.</param>
    /// <returns>A base64-encoded string representing the Excel (.xlsx) file.</returns>
    string ExportToExcel(Document document);
}
