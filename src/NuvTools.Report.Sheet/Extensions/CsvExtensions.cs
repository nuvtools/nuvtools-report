using ClosedXML.Excel;
using NuvTools.Report.Table.Models;

namespace NuvTools.Report.Sheet.Extensions;

/// <summary>
/// Provides extension methods for exporting documents to CSV format.
/// </summary>
public static class CsvExtensions
{
    /// <summary>
    /// Exports all tables in a document to separate CSV files encoded as base64 strings.
    /// </summary>
    /// <param name="document">The document containing tables to export.</param>
    /// <param name="delimiter">The delimiter to use. Defaults to <see cref="CsvDelimiter.Comma"/>.</param>
    /// <param name="customDelimiter">A custom delimiter string, required when <paramref name="delimiter"/> is <see cref="CsvDelimiter.Custom"/>.</param>
    /// <returns>A list of base64-encoded CSV strings, one for each table in the document.</returns>
    /// <remarks>
    /// CSV files use comma (,) as the default delimiter per RFC 4180. Headers are excluded from the export.
    /// </remarks>
    public static List<string> ExportToCsv(this Document document,
        CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null)
    {
        var delimiterString = delimiter.ToDelimiterString(customDelimiter);

        var xlWbook = document.BuildWorkbook(false);

        var worksheets = xlWbook.BuildCsvList(delimiterString);

        var stringBase64List = new List<string>();

        foreach (var sheet in worksheets)
            stringBase64List.Add(ConvertToBase64String(sheet));

        return stringBase64List;
    }

    /// <summary>
    /// Exports the first table in a document to a CSV file encoded as a base64 string.
    /// </summary>
    /// <param name="document">The document containing the table to export.</param>
    /// <param name="delimiter">The delimiter to use. Defaults to <see cref="CsvDelimiter.Comma"/>.</param>
    /// <param name="customDelimiter">A custom delimiter string, required when <paramref name="delimiter"/> is <see cref="CsvDelimiter.Custom"/>.</param>
    /// <returns>A base64-encoded CSV string of the first table.</returns>
    /// <remarks>
    /// CSV uses comma (,) as the default delimiter per RFC 4180. Headers are excluded from the export.
    /// </remarks>
    public static string ExportFirstSheetToCsv(this Document document,
        CsvDelimiter delimiter = CsvDelimiter.Comma, string? customDelimiter = null)
    {
        var delimiterString = delimiter.ToDelimiterString(customDelimiter);

        var xlWbook = document.BuildWorkbook(false);

        var lines = xlWbook.Worksheets.First().BuildCsvList(delimiterString);

        return ConvertToBase64String(lines);
    }

    /// <summary>
    /// Converts all worksheets in an Excel workbook to CSV format.
    /// </summary>
    /// <param name="xlWorkbook">The Excel workbook to convert.</param>
    /// <param name="delimiter">The delimiter string to separate values.</param>
    /// <returns>A list of lists where each inner list represents CSV lines for a worksheet.</returns>
    private static List<List<string>> BuildCsvList(this XLWorkbook xlWorkbook, string delimiter)
    {
        var worksheets = new List<List<string>>();

        foreach (var xlWorksheet in xlWorkbook.Worksheets)
            worksheets.Add(xlWorksheet.BuildCsvList(delimiter));

        return worksheets;
    }

    /// <summary>
    /// Converts a single worksheet to CSV format.
    /// </summary>
    /// <param name="xLWorksheet">The worksheet to convert.</param>
    /// <param name="delimiter">The delimiter string to separate values.</param>
    /// <returns>A list of strings where each string is a CSV line with delimiter-separated values.</returns>
    private static List<string> BuildCsvList(this IXLWorksheet xLWorksheet, string delimiter)
    {
        var lines = xLWorksheet.RowsUsed().Select(row =>
        string.Join(delimiter, row.Cells(1, row.CellsUsed(XLCellsUsedOptions.Contents).Count())
                        .Select(cell => cell.GetValue<string>()))).ToList();

        return lines;
    }


    /// <summary>
    /// Converts a list of CSV lines to a base64-encoded string.
    /// </summary>
    /// <param name="lines">The CSV lines to convert.</param>
    /// <returns>A base64-encoded string representing the CSV content.</returns>
    private static string ConvertToBase64String(List<string> lines)
    {
        using MemoryStream ms = new();
        var sw = new StreamWriter(ms);

        lines.ForEach(a => sw.WriteLine(a));
        sw.Flush();

        return Convert.ToBase64String(ms.ToArray());
    }

}