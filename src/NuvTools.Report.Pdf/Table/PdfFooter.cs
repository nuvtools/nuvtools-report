using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfFooter(Report.Table.Models.Table model) : IComponent
{
    private Report.Table.Models.Table Model { get; set; } = model;

    public void Compose(IContainer container)
    {
        container
            .PaddingBottom(10)
            .PaddingHorizontal(22)
            .Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text($"{Model.Info.IssueUser.ToUpper()} - {DateTime.Now}")
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