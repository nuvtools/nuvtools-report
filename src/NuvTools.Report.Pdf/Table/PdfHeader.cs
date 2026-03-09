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
    /// determined by the table's Style.BackgroundHeaderColor property.
    /// </remarks>
    public void Compose(IContainer container)
    {
        container
            .Height(60)
            .Background(model.Style!.BackgroundHeaderColor)
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
                    if (!string.IsNullOrEmpty(model.Info.LogoImage))
                    {
                        column.Item().Image(Convert.FromBase64String(model.Info.LogoImage)).FitWidth();

                        column.Item()
                            .PaddingTop(2)
                            .Hyperlink(model.Info.CompanyUrl ?? string.Empty)
                            .Text(model.Info.CompanyUrl?.ToUpper() ?? string.Empty)
                            .FontSize(11).Bold().FontColor(Colors.White);
                    }
                    else
                    {
                        column.Item().PaddingTop(10)
                            .Text(model.Info.CompanyAbbreviation?.ToUpper() ?? string.Empty)
                            .FontSize(17).Bold().FontColor(Colors.White);

                        column.Item().PaddingTop(-4)
                            .Hyperlink(model.Info.CompanyUrl ?? string.Empty)
                            .Text(model.Info.CompanyUrl?.ToLower() ?? string.Empty)
                            .FontSize(11).Bold().FontColor(Colors.White);
                    }
                });
            });
    }

}
