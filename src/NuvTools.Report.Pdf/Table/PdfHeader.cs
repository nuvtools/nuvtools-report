using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

/// <summary>
/// Renders the header section of a PDF page, displaying the report title, filter description, and company information.
/// </summary>
internal class PdfHeader(NuvTools.Report.Table.Models.Table model) : IComponent
{
    /// <summary>
    /// Composes the header layout with report title, filter description, and company branding.
    /// </summary>
    /// <param name="container">The container to render the header into.</param>
    /// <remarks>
    /// The header displays the report title and filter description on the left side in white text.
    /// On the right side, it shows either the company logo (if provided as base64) with URL,
    /// or the company abbreviation with a clickable URL hyperlink. The background color is
    /// determined by the table's Style.BackgroundHeaderColor property, or a dark grey when the table
    /// has no style: the text is white, so the header always needs a dark background.
    /// </remarks>
    public void Compose(IContainer container)
    {
        Color background = model.Style is null || string.IsNullOrEmpty(model.Style.BackgroundHeaderColor)
            ? Colors.Grey.Darken3
            : model.Style.BackgroundHeaderColor;

        container
            .Height(60)
            .Background(background)
            .PaddingHorizontal(16)
            .Row(row =>
            {
                row.RelativeItem(10).Column(column =>
                {
                    column.Item().PaddingTop(9)
                        .Text(model.Info.Title?.ToUpper() ?? string.Empty)
                        .FontSize(18).Bold().FontColor(Colors.White);

                    column.Item().PaddingTop(-5)
                        .Text(model.Info.FilterDescription?.ToUpper() ?? string.Empty)
                        .FontSize(10).Bold().FontColor(Colors.White);
                });

                row.ConstantItem(140).AlignRight().Column(column =>
                {
                    // QuestPDF refuses an empty hyperlink, so the URL line exists only when there is one.
                    var url = model.Info.CompanyUrl;

                    if (!string.IsNullOrEmpty(model.Info.LogoImage))
                    {
                        column.Item().Image(Convert.FromBase64String(model.Info.LogoImage)).FitWidth();

                        if (!string.IsNullOrWhiteSpace(url))
                            column.Item()
                                .PaddingTop(2)
                                .Hyperlink(url)
                                .Text(url.ToUpper())
                                .FontSize(11).Bold().FontColor(Colors.White);
                    }
                    else
                    {
                        column.Item().PaddingTop(10)
                            .Text(model.Info.CompanyAbbreviation?.ToUpper() ?? string.Empty)
                            .FontSize(17).Bold().FontColor(Colors.White);

                        if (!string.IsNullOrWhiteSpace(url))
                            column.Item().PaddingTop(-4)
                                .Hyperlink(url)
                                .Text(url.ToLower())
                                .FontSize(11).Bold().FontColor(Colors.White);
                    }
                });
            });
    }

}
