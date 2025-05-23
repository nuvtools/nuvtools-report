using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfHeader(NuvTools.Report.Table.Models.Table model) : IComponent
{
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
                        .Text(model.Info.Title.ToUpper())
                        .FontSize(18).Bold().FontColor(Colors.White);

                    column.Item().PaddingTop(-5)
                        .Text(model.Info.FilterDescription.ToUpper())
                        .FontSize(10).Bold().FontColor(Colors.White);
                });

                row.ConstantItem(140).AlignRight().Column(column =>
                {
                    if (!string.IsNullOrEmpty(model.Info.LogoImage))
                    {
                        column.Item().Image(Convert.FromBase64String(model.Info.LogoImage)).FitWidth();

                        column.Item()
                            .PaddingTop(2)
                            .Hyperlink(model.Info.CompanyUrl)
                            .Text(model.Info.CompanyUrl.ToUpper())
                            .FontSize(11).Bold().FontColor(Colors.White);
                    }
                    else
                    {
                        column.Item().PaddingTop(10)
                            .Text(model.Info.CompanyAbbreviation.ToUpper())
                            .FontSize(17).Bold().FontColor(Colors.White);

                        column.Item().PaddingTop(-4)
                            .Hyperlink(model.Info.CompanyUrl)
                            .Text(model.Info.CompanyUrl.ToLower())
                            .FontSize(11).Bold().FontColor(Colors.White);
                    }
                });
            });
    }

}
