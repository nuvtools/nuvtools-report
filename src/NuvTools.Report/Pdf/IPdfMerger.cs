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
    /// <exception cref="ArgumentException">
    /// No page to merge: the collection is empty or its documents have no pages. A PDF must have at
    /// least one page, so there is no valid empty result to return.
    /// </exception>
    byte[] Merge(IEnumerable<byte[]> pdfs);
}
