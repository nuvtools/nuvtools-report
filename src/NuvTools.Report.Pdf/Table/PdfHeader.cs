using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfHeader : IComponent
{
    private NuvTools.Report.Table.Models.Table Model { get; set; }

    public PdfHeader(NuvTools.Report.Table.Models.Table model)
    {
        Model = model;
    }

    public void Compose(IContainer container)
    {
        container.Height(60).Background(Model.Style.BackgroundHeaderColor).Grid(grid =>
        {
            grid.Item(10).Column(column =>
            {
                column.Item().PaddingTop(9)
                .PaddingLeft(18)
                .Text(Model.Info.Title.ToUpper())
                .FontSize(18).Bold().FontColor(Colors.White);

                column.Item()
                .PaddingLeft(18).AlignLeft().PaddingTop(-5)
                .Text(Model.Info.FilterDescription.ToUpper())
                .FontSize(10).Bold().FontColor(Colors.White);
            });

            if (!string.IsNullOrEmpty(Model.Info.LogoImage))
            {
                grid.Item(2).Background(Model.Style.BackgroundHeaderColor)
                .AlignRight()
                .PaddingTop(18)
                .PaddingRight(16)
                .Width(70)
                .Column(stackImage =>
                {
                    stackImage.Item().Image(Convert.FromBase64String(Model.Info.LogoImage), ImageScaling.FitWidth);
                    stackImage.Item()
                   .Hyperlink(Model.Info.CompanyUrl)
                   .Text(Model.Info.CompanyUrl.ToUpper())
                   .FontSize(11)
                   .Bold().FontColor(Colors.White);
                });
            }
            else
            {
                grid.Item(2).PaddingRight(16).AlignRight().Column(columnLogo =>
                {

                    columnLogo.Item()
                    .AlignRight()
                    .PaddingTop(10)
                    .Text(Model.Info.CompanyAbbreviation.ToUpper())
                    .FontSize(17).Bold().FontColor(Colors.White);

                    columnLogo.Item()
                    .AlignRight()
                    .PaddingTop(-4)
                    .Hyperlink(Model.Info.CompanyUrl)
                    .Text(Model.Info.CompanyUrl.ToLower())
                    .FontSize(11).Bold().FontColor(Colors.White);
                });
            }
        });
    }
}
