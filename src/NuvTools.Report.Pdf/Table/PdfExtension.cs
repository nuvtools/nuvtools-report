using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

public static class PdfExtension
{
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

    public static string ExportFirstSheetToPdf(this Report.Table.Models.Document document)
    {
        var pdfSheet = new PdfSheet(document.Tables[0]);
        return pdfSheet.GeneratePdfAsString64();
    }

    public static string GeneratePdfAsString64(this IDocument document)
    {
        byte[] bytes = document.GeneratePdf();
        return Convert.ToBase64String(bytes);
    }
}
