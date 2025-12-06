namespace NuvTools.Report.Table.Models;

/// <summary>
/// Represents a report document containing one or more tables that can be exported to various formats (PDF, Excel, CSV).
/// </summary>
public class Document
{
    /// <summary>
    /// Gets or sets the background color for the document header.
    /// </summary>
    /// <remarks>
    /// Color should be in HTML color format (e.g., "#FFFFFF" or "white").
    /// </remarks>
    public string BackgroundDocumentHeaderColor { get; set; } = "#FFFFFF";

    /// <summary>
    /// Gets or sets the collection of tables contained in this document.
    /// </summary>
    /// <remarks>
    /// Each table can be rendered as a separate worksheet in Excel or as separate pages in PDF.
    /// </remarks>
    public List<Table> Tables { get; set; } = [];
}
