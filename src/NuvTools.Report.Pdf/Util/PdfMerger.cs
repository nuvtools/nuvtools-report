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
        ArgumentNullException.ThrowIfNull(pdfs);

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

        // A PDF must have at least one page; PDFsharp refuses to save an empty one, so say why here.
        if (output.PageCount == 0)
            throw new ArgumentException("There is nothing to merge: no PDF with at least one page was given.", nameof(pdfs));

        using var ms = new MemoryStream();
        output.Save(ms);
        return ms.ToArray();
    }
}
