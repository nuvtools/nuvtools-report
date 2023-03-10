using ClosedXML.Excel;
using NuvTools.Report.Table.Models;

namespace NuvTools.Report.Sheet.Extensions;

public static class CsvExtensions
{
    public static List<string> ExportToCsv(this Document document)
    {
        var xlWbook = document.BuildWorkbook(false);

        var worksheets = xlWbook.BuildCsvList();

        var stringBase64List = new List<string>();

        foreach (var sheet in worksheets)
            stringBase64List.Add(ConvertToBase64String(sheet));

        return stringBase64List;
    }

    public static string ExportFirstSheetToCsv(this Document document)
    {
        var xlWbook = document.BuildWorkbook(false);

        var lines = xlWbook.Worksheets.First().BuildCsvList();

        return ConvertToBase64String(lines);
    }

    private static List<List<string>> BuildCsvList(this XLWorkbook xlWorkbook)
    {
        var worksheets = new List<List<string>>();

        foreach (var xlWorksheet in xlWorkbook.Worksheets)
            worksheets.Add(xlWorksheet.BuildCsvList());

        return worksheets;
    }

    private static List<string> BuildCsvList(this IXLWorksheet xLWorksheet)
    {
        var lines = xLWorksheet.RowsUsed().Select(row =>
        string.Join(";", row.Cells(1, row.CellsUsed(XLCellsUsedOptions.Contents).Count())
                        .Select(cell => cell.GetValue<string>()))).ToList();

        return lines;
    }


    private static string ConvertToBase64String(List<string> lines)
    {
        using MemoryStream ms = new();
        var sw = new StreamWriter(ms);

        lines.ForEach(a => sw.WriteLine(a));
        sw.Flush();

        return Convert.ToBase64String(ms.ToArray());
    }

}