using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

/// <summary>
/// Represents a PDF document that renders a single table with header, content, and footer.
/// </summary>
internal class PdfSheet : IDocument
{
    /// <summary>
    /// Gets the table model that this PDF document represents.
    /// </summary>
    public NuvTools.Report.Table.Models.Table Model { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfSheet"/> class.
    /// </summary>
    /// <param name="model">The table model to render as a PDF.</param>
    public PdfSheet(NuvTools.Report.Table.Models.Table model)
    {
        Model = model;
    }

    /// <summary>
    /// Gets the metadata for this PDF document.
    /// </summary>
    /// <returns>Default document metadata.</returns>
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <summary>
    /// Composes the PDF document layout with header, content table, and footer.
    /// </summary>
    /// <param name="container">The document container to compose into.</param>
    /// <remarks>
    /// The page is rendered in landscape A4 format with header, table content, and footer sections.
    /// </remarks>
    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeTable);
                page.Footer().Element(ComposeFooter);
            });
    }

    /// <summary>
    /// Composes the header section of the PDF page.
    /// </summary>
    /// <param name="container">The container for the header.</param>
    void ComposeHeader(IContainer container)
    {
        container.Component<PdfHeader>(new(Model));
    }

    /// <summary>
    /// Composes the footer section of the PDF page.
    /// </summary>
    /// <param name="container">The container for the footer.</param>
    void ComposeFooter(IContainer container)
    {
        container.Component<PdfFooter>(new(Model));
    }

    /// <summary>
    /// Composes the table content section of the PDF page.
    /// </summary>
    /// <param name="container">The container for the table.</param>
    private void ComposeTable(IContainer container)
    {
        container.Component<PdfTable>(new(Model));
    }
}