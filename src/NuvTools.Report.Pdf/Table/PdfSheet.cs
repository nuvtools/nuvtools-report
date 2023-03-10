using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfSheet : IDocument
{
    public NuvTools.Report.Table.Models.Table Model { get; }

    public PdfSheet(NuvTools.Report.Table.Models.Table model)
    {
        Model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

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

    void ComposeHeader(IContainer container)
    {
        container.Component<PdfHeader>(new(Model));
    }

    void ComposeFooter(IContainer container)
    {
        container.Component<PdfFooter>(new(Model));
    }

    private void ComposeTable(IContainer container)
    {
        container.Component<PdfTable>(new(Model));
    }
}