using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace NuvTools.Report.Pdf.Util;

public static class Pdf
{
    public static byte[] Merge(IEnumerable<byte[]> pdfs)
    {
        using var writerMemoryStream = new MemoryStream();
        using (var writer = new PdfWriter(writerMemoryStream))
        {
            using var mergedDocument = new PdfDocument(writer);
            var merger = new PdfMerger(mergedDocument);

            foreach (var pdfBytes in pdfs)
            {
                using var copyFromMemoryStream = new MemoryStream(pdfBytes);
                using var reader = new PdfReader(copyFromMemoryStream);
                using var copyFromDocument = new PdfDocument(reader);
                merger.Merge(copyFromDocument, 1, copyFromDocument.GetNumberOfPages());
            }
        }

        return writerMemoryStream.ToArray();
    }
}
