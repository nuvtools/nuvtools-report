using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace NuvTools.Report.Pdf.Util;

/// <summary>
/// Provides utility methods for working with PDF documents.
/// </summary>
public static class PdfUtil
{
    /// <summary>
    /// Merges multiple PDF documents into a single PDF document.
    /// </summary>
    /// <param name="pdfs">An enumerable collection of PDF documents represented as byte arrays.</param>
    /// <returns>A byte array representing the merged PDF document.</returns>
    /// <remarks>
    /// All pages from each input PDF are combined sequentially into the output document.
    /// The order of pages in the output matches the order of PDFs in the input collection.
    /// </remarks>
    public static byte[] Merge(IEnumerable<byte[]> pdfs)
    {
        using var output = new PdfDocument();

        foreach (var pdfBytes in pdfs)
        {
            using var inputStream = new MemoryStream(pdfBytes);
            using var inputDocument = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);

            for (int pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
            {
                output.AddPage(inputDocument.Pages[pageIndex]);
            }
        }

        using var ms = new MemoryStream();
        output.Save(ms);
        return ms.ToArray();
    }
}