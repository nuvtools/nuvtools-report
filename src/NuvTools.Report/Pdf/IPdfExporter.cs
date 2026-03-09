using NuvTools.Report.Table.Models;

namespace NuvTools.Report.Pdf;

/// <summary>
/// Exports documents to PDF format encoded as base64 strings.
/// </summary>
public interface IPdfExporter
{
    /// <summary>
    /// Exports all tables in a document to separate PDF files encoded as base64 strings.
    /// </summary>
    /// <param name="document">The document containing tables to export.</param>
    /// <returns>A list of base64-encoded PDF strings, one for each table in the document.</returns>
    List<string> ExportSheetToPdf(Document document);

    /// <summary>
    /// Exports the first table in a document to a PDF file encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing the table to export.</param>
    /// <returns>A base64-encoded PDF string of the first table.</returns>
    string ExportFirstSheetToPdf(Document document);
}
