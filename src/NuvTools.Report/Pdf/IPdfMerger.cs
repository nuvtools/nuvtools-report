namespace NuvTools.Report.Pdf;

/// <summary>
/// Merges multiple PDF documents into a single PDF document.
/// </summary>
public interface IPdfMerger
{
    /// <summary>
    /// Merges multiple PDF documents into a single PDF document.
    /// </summary>
    /// <param name="pdfs">An enumerable collection of PDF documents represented as byte arrays.</param>
    /// <returns>A byte array representing the merged PDF document.</returns>
    byte[] Merge(IEnumerable<byte[]> pdfs);
}
