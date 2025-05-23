using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfTable(NuvTools.Report.Table.Models.Table model) : IComponent
{
    public void Compose(IContainer container)
    {
        container
            .PaddingTop(10)
            .PaddingHorizontal(22)
            .Decoration(decoration =>
            {
                // Header
                decoration.Before()
                    .BorderBottom(1)
                    .Padding(5)
                    .Row(row =>
                    {
                        foreach (var item in model.Content.Header.Columns.OrderBy(a => a.Order))
                        {
                            row.RelativeItem().AlignCenter().Text(item.Label);
                        }
                    });

                // Content
                decoration.Content().Column(column =>
                {
                    foreach (var item in model.Content.Rows)
                    {
                        column.Item()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten3)
                            .Padding(4)
                            .Row(row =>
                            {
                                foreach (var cell in item.Cells)
                                {
                                    if (DateTime.TryParse(cell.Value, out DateTime date))
                                    {
                                        row.RelativeItem().AlignCenter().Text(date.ToString(cell.Column.Format));
                                    }
                                    else
                                    {
                                        row.RelativeItem().AlignCenter().Text(cell.Value);
                                    }
                                }
                            });
                    }
                });
            });
    }


}
