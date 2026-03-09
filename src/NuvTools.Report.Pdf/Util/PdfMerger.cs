using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace NuvTools.Report.Pdf.Util;

/// <summary>
/// Merges multiple PDF documents into a single PDF document using PDFsharp.
/// </summary>
public class PdfMerger : IPdfMerger
{
    /// <inheritdoc />
    public byte[] Merge(IEnumerable<byte[]> pdfs)
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
