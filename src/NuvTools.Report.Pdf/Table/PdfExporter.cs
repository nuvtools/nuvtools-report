using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

/// <summary>
/// Exports documents to PDF format using QuestPDF.
/// </summary>
public class PdfExporter : IPdfExporter
{
    /// <inheritdoc />
    public List<string> ExportSheetToPdf(NuvTools.Report.Table.Models.Document document)
    {
        var worksheets = new List<string>();

        foreach (var worksheet in document.Tables)
        {
            var pdfSheet = new PdfSheet(worksheet);
            worksheets.Add(GeneratePdfAsString64(pdfSheet));
        }

        return worksheets;
    }

    /// <inheritdoc />
    public string ExportFirstSheetToPdf(NuvTools.Report.Table.Models.Document document)
    {
        var pdfSheet = new PdfSheet(document.Tables[0]);
        return GeneratePdfAsString64(pdfSheet);
    }

    /// <summary>
    /// Converts a QuestPDF document to a base64-encoded string.
    /// </summary>
    /// <param name="document">The QuestPDF document to convert.</param>
    /// <returns>A base64-encoded string representation of the PDF.</returns>
    private static string GeneratePdfAsString64(IDocument document)
    {
        byte[] bytes = document.GeneratePdf();
        return Convert.ToBase64String(bytes);
    }
}
