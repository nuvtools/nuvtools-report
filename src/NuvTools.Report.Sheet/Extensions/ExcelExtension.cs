using ClosedXML.Excel;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Sheet.Extensions;

/// <summary>
/// Provides extension methods for exporting documents to Excel format.
/// </summary>
public static class ExcelExtension
{
    /// <summary>
    /// Exports a document to Excel (.xlsx) format encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing tables to export.</param>
    /// <returns>A base64-encoded string representing the Excel (.xlsx) file.</returns>
    /// <remarks>
    /// Each table in the document becomes a separate worksheet in the Excel file.
    /// The export includes full headers with company information, titles, and filters.
    /// </remarks>
    public static string ExportToExcel(this Document document)
    {
        var xlWbook = document.BuildWorkbook(true);

        using MemoryStream ms = new();

        xlWbook.SaveAs(ms);

        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Builds an Excel workbook from a document.
    /// </summary>
    /// <param name="document">The document to convert.</param>
    /// <param name="includeHeader">Whether to include report headers with company info and titles.</param>
    /// <returns>An Excel workbook containing all tables as worksheets.</returns>
    internal static XLWorkbook BuildWorkbook(this Document document, bool includeHeader)
    {
        var xlWbook = new XLWorkbook();

        foreach (var worksheet in document.Tables)
            xlWbook.AddWorksheet(worksheet, includeHeader);

        return xlWbook;
    }

    /// <summary>
    /// Adds a table as a worksheet to an Excel workbook.
    /// </summary>
    /// <param name="xlWbook">The workbook to add the worksheet to.</param>
    /// <param name="worksheet">The table to add as a worksheet.</param>
    /// <param name="includeHeader">Whether to include report headers with company info and titles.</param>
    /// <remarks>
    /// When includeHeader is true, the worksheet includes title, company info, filter description,
    /// and issue user/date before the data table. The header uses custom styling from the table's Style property.
    /// </remarks>
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
                    BackgroundHeaderColor = worksheet.Style!.BackgroundHeaderColor,
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

    /// <summary>
    /// Adds the column header row to a worksheet.
    /// </summary>
    /// <param name="xlWorksheet">The worksheet to add the header to.</param>
    /// <param name="index">The row index where the header should be placed.</param>
    /// <param name="worksheet">The table containing column definitions.</param>
    private static void AddContentHeader(this IXLWorksheet xlWorksheet, int index, Table.Models.Table worksheet)
    {
        var style = new Style
        {
            BackgroundLineGray = true
        };

        xlWorksheet.AddValuesRow(index, PivotColumnstoCells(worksheet.Content.Header.Columns), style);
    }

    /// <summary>
    /// Adds a single text row to a worksheet (used for titles and header information).
    /// </summary>
    /// <param name="xlWorksheet">The worksheet to add text to.</param>
    /// <param name="xlRowNumber">The row number where the text should be placed.</param>
    /// <param name="text">The text content to add.</param>
    /// <param name="style">The styling to apply to the text.</param>
    private static void AddWorksheetText(this IXLWorksheet xlWorksheet, int xlRowNumber, string text, Style style)
    {
        var cell = new List<Cell> { new() { Column = new Column { Order = 1 }, Value = text } };
        xlWorksheet.AddValuesRow(xlRowNumber, cell, style);
    }

    /// <summary>
    /// Converts a list of columns to a list of cells for header rendering.
    /// </summary>
    /// <param name="columns">The columns to convert.</param>
    /// <returns>A list of cells containing column labels.</returns>
    private static List<Cell> PivotColumnstoCells(List<Column> columns)
    {
        return columns.Select(a => new Cell
        {
            Value = a.Label,
            Column = a
        }).ToList();
    }

    /// <summary>
    /// Adds a row of cell values to a worksheet with optional styling.
    /// </summary>
    /// <param name="xlWorksheet">The worksheet to add the row to.</param>
    /// <param name="xlRowNumber">The row number where values should be placed.</param>
    /// <param name="cells">The cells to add to the row.</param>
    /// <param name="style">Optional styling to apply to the cells.</param>
    /// <remarks>
    /// Cells are ordered by their Column.Order property. DateTime values are formatted using
    /// the Column.Format property. When BackgroundHeaderColor is set, the row spans 40 columns.
    /// </remarks>
    private static void AddValuesRow(this IXLWorksheet xlWorksheet, int xlRowNumber, List<Cell> cells, Style? style)
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

    /// <summary>
    /// Applies styling properties to an Excel cell.
    /// </summary>
    /// <param name="cell">The cell to style.</param>
    /// <param name="style">The style properties to apply.</param>
    /// <remarks>
    /// Applies bold, font size, background colors (gray or custom HTML color), and font colors.
    /// If style is null, no styling is applied. HTML colors should be in format "#RRGGBB".
    /// </remarks>
    private static void SetCellStyle(this IXLCell cell, Style? style)
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
