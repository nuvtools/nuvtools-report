using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

/// <summary>
/// Provides extension methods for exporting documents and tables to PDF format.
/// </summary>
public static class PdfExtension
{
    /// <summary>
    /// Exports all tables in a document to separate PDF files encoded as base64 strings.
    /// </summary>
    /// <param name="document">The document containing tables to export.</param>
    /// <returns>A list of base64-encoded PDF strings, one for each table in the document.</returns>
    /// <remarks>
    /// Each table is rendered as a separate PDF document in landscape A4 format.
    /// </remarks>
    public static List<string> ExportSheetToPdf(this NuvTools.Report.Table.Models.Document document)
    {
        var worksheets = new List<string>();

        foreach (var worksheet in document.Tables)
        {
            var pdfSheet = new PdfSheet(worksheet);
            worksheets.Add(pdfSheet.GeneratePdfAsString64());
        }

        return worksheets;
    }

    /// <summary>
    /// Exports the first table in a document to a PDF file encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing the table to export.</param>
    /// <returns>A base64-encoded PDF string of the first table.</returns>
    /// <remarks>
    /// Useful when working with single-table documents. The PDF is rendered in landscape A4 format.
    /// </remarks>
    public static string ExportFirstSheetToPdf(this Report.Table.Models.Document document)
    {
        var pdfSheet = new PdfSheet(document.Tables[0]);
        return pdfSheet.GeneratePdfAsString64();
    }

    /// <summary>
    /// Converts a QuestPDF document to a base64-encoded string.
    /// </summary>
    /// <param name="document">The QuestPDF document to convert.</param>
    /// <returns>A base64-encoded string representation of the PDF.</returns>
    public static string GeneratePdfAsString64(this IDocument document)
    {
        byte[] bytes = document.GeneratePdf();
        return Convert.ToBase64String(bytes);
    }
}
