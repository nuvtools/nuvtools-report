using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

/// <summary>
/// Renders the footer section of a PDF page, displaying the issue user, timestamp, and page numbers.
/// </summary>
internal class PdfFooter(Report.Table.Models.Table model) : IComponent
{
    /// <summary>
    /// Gets or sets the table model containing information for the footer.
    /// </summary>
    private Report.Table.Models.Table Model { get; set; } = model;

    /// <summary>
    /// Composes the footer layout with user information and pagination.
    /// </summary>
    /// <param name="container">The container to render the footer into.</param>
    /// <remarks>
    /// The footer displays the issue user name and current timestamp on the left,
    /// and page numbers (current/total) on the right in a light gray color.
    /// </remarks>
    public void Compose(IContainer container)
    {
        container
            .PaddingBottom(10)
            .PaddingHorizontal(22)
            .Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text($"{Model.Info.IssueUser?.ToUpper()} - {DateTime.Now}")
                    .FontSize(9)
                    .FontColor("#AAAAAA");

                row.ConstantItem(100).AlignRight().AlignMiddle().Text(text =>
                {
                    text.DefaultTextStyle(TextStyle.Default.FontSize(10).FontColor("#AAAAAA"));
                    text.CurrentPageNumber();
                    text.Span("/");
                    text.TotalPages();
                });
            });
    }
}