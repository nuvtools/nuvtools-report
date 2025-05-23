using ClosedXML.Excel;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Sheet.Extensions;

public static class ExcelExtension
{
    /// <summary>
    /// Export document to Excel format.
    /// </summary>
    /// <param name="document">Document data.</param>
    /// <returns>Excel (.xlsx) file in base 64 string.</returns>
    public static string ExportToExcel(this Document document)
    {
        var xlWbook = document.BuildWorkbook(true);

        using MemoryStream ms = new();

        xlWbook.SaveAs(ms);

        return Convert.ToBase64String(ms.ToArray());
    }

    internal static XLWorkbook BuildWorkbook(this Document document, bool includeHeader)
    {
        var xlWbook = new XLWorkbook();

        foreach (var worksheet in document.Tables)
            xlWbook.AddWorksheet(worksheet, includeHeader);

        return xlWbook;
    }

    public static void AddWorksheet(this XLWorkbook xlWbook, Table.Models.Table worksheet, bool includeHeader)
    {
        var rowIndex = 1;

        var xlWorksheet = xlWbook.Worksheets.Add(worksheet.Info.Name, worksheet.Info.Order);

        if (includeHeader)
        {
            xlWorksheet.AddWorksheetText(rowIndex, worksheet.Info.Title,
                new Style
                {
                    Bold = true,
                    FontSize = 14.0,
                    BackgroundHeaderColor = worksheet.Style.BackgroundHeaderColor,
                    FontHeaderColor = worksheet.Style.FontHeaderColor
                });

            xlWorksheet.AddWorksheetText(++rowIndex,
                $"{worksheet.Info.CompanyAbbreviation} ({worksheet.Info.CompanyUrl})",
                new Style
                {
                    FontSize = 12,
                    BackgroundHeaderColor = worksheet.Style.BackgroundHeaderColor,
                    FontHeaderColor = worksheet.Style.FontHeaderColor
                });

            rowIndex++; ;

            xlWorksheet.AddWorksheetText(++rowIndex, worksheet.Info.FilterDescription, new Style { FontSize = 10 });
            xlWorksheet.AddWorksheetText(++rowIndex,
                $"{worksheet.Info.IssueUser} - {worksheet.Info.IssueDate}",
                new Style { FontSize = 10 });

            rowIndex += 2;
        }

        xlWorksheet.AddContentHeader(rowIndex, worksheet);

        rowIndex++;

        foreach (var row in worksheet.Content.Rows)
        {
            xlWorksheet.AddValuesRow(rowIndex, row.Cells, null);
            rowIndex++;
        }
    }

    private static void AddContentHeader(this IXLWorksheet xlWorksheet, int index, Table.Models.Table worksheet)
    {
        var style = new Style
        {
            BackgroundLineGray = true
        };

        xlWorksheet.AddValuesRow(index, PivotColumnstoCells(worksheet.Content.Header.Columns), style);
    }

    private static void AddWorksheetText(this IXLWorksheet xlWorksheet, int xlRowNumber, string text, Style style)
    {
        var cell = new List<Cell> { new() { Column = new Column { Order = 1 }, Value = text } };
        xlWorksheet.AddValuesRow(xlRowNumber, cell, style);
    }

    private static List<Cell> PivotColumnstoCells(List<Column> columns)
    {
        return columns.Select(a => new Cell
        {
            Value = a.Label,
            Column = a
        }).ToList();
    }

    private static void AddValuesRow(this IXLWorksheet xlWorksheet, int xlRowNumber, List<Cell> cells, Style style)
    {
        var totalColumns = cells.Max(a => a.Column.Order);

        if (totalColumns <= 1)
            totalColumns = (short)cells.Count;
        else
            cells = cells.OrderBy(a => a.Column.Order).ToList();

        if (style != null && !string.IsNullOrEmpty(style.BackgroundHeaderColor))
            totalColumns = 40;

        var xlColumns = xlWorksheet.Columns(1, totalColumns);

        var index = 0;

        foreach (var xlColumn in xlColumns)
        {
            if (index < cells.Count)
            {
                if (DateTime.TryParse(cells[index].Value, out DateTime date))
                {
                    xlColumn.Cell(xlRowNumber).Value = date.ToString(cells[index].Column.Format);
                }
                else
                {
                    xlColumn.Cell(xlRowNumber).Value = cells[index].Value;
                }
            }

            xlColumn.Cell(xlRowNumber).SetCellStyle(style);

            index++;
        }
    }

    private static void SetCellStyle(this IXLCell cell, Style style)
    {
        if (style == null) return;

        if (style.Bold.HasValue)
        {
            if (!cell.HasRichText)
                cell.CreateRichText();

            cell.GetRichText().SetBold(style.Bold.Value);
        }

        if (style.FontSize.HasValue)
        {
            if (!cell.HasRichText)
                cell.CreateRichText();

            cell.GetRichText().SetFontSize(style.FontSize.Value);
        }

        if (style.BackgroundLineGray.HasValue)
            cell.Style.Fill.SetBackgroundColor(XLColor.LightGray);

        if (!string.IsNullOrEmpty(style.BackgroundHeaderColor))
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(style.BackgroundHeaderColor));

        if (!string.IsNullOrEmpty(style.FontHeaderColor))
            cell.Style.Font.SetFontColor(XLColor.FromHtml(style.FontHeaderColor));
    }
}
