using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfFooter : IComponent
{
    private NuvTools.Report.Table.Models.Table Model { get; set; }

    public PdfFooter(NuvTools.Report.Table.Models.Table model)
    {
        Model = model;
    }

    public void Compose(IContainer container)
    {
        container.PaddingBottom(10)


            .Grid(grid =>
            {
                grid.AlignCenter();

                grid.Item(10)
                    .PaddingLeft(22)
                    .Text($"{Model.Info.IssueUser.ToUpper()} - {DateTime.Now}")
                    .FontSize(9)
                    .FontColor("#AAAAAA");

                grid.Item(2)
                .AlignRight()
                .PaddingRight(22)
                .Text(text =>
                {
                    text.DefaultTextStyle(TextStyle.Default.FontSize(10).FontColor("#AAAAAA"));
                    text.CurrentPageNumber();
                    text.Span("/");
                    text.TotalPages();
                });
            });
    }
}