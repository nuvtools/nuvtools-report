using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NuvTools.Report.Pdf.Table;

internal class PdfTable : IComponent
{
    private NuvTools.Report.Table.Models.Table Model { get; set; }

    public PdfTable(NuvTools.Report.Table.Models.Table model)
    {
        Model = model;
    }

    public void Compose(IContainer container)
    {
        container.PaddingTop(10).PaddingLeft(22).PaddingRight(22).Decoration(decoration =>
        {
            // header
            decoration.Before().BorderBottom(1).Padding(5).Row(row =>
            {
                foreach (var item in Model.Content.Header.Columns.OrderBy(a => a.Order))
                {
                    row.RelativeItem().AlignCenter().Text(item.Label);
                }
            });

            // content
            decoration
            .Content()
            .Column(column =>
            {
                foreach (var item in Model.Content.Rows)
                {
                    column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Row(row =>
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
