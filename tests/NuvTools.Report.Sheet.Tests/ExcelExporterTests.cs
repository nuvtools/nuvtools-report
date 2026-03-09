using ClosedXML.Excel;
using NUnit.Framework;
using NuvTools.Report.Sheet.Excel;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Sheet.Tests;

[TestFixture]
public class ExcelExporterTests
{
    private ExcelExporter _exporter = null!;

    [SetUp]
    public void SetUp()
    {
        _exporter = new ExcelExporter();
    }

    [Test]
    public void ExportToExcel_SingleTable_ReturnsSingleWorksheet()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", 1, [["A1", "B1"], ["A2", "B2"]])
        ]);

        var base64 = _exporter.ExportToExcel(document);
        using var workbook = OpenWorkbook(base64);

        Assert.That(workbook.Worksheets, Has.Count.EqualTo(1));
    }

    [Test]
    public void ExportToExcel_MultipleTables_CreatesOneWorksheetPerTable()
    {
        var document = CreateDocument(
        [
            CreateTable("First", 1, [["A1"]]),
            CreateTable("Second", 2, [["B1"]])
        ]);

        var base64 = _exporter.ExportToExcel(document);
        using var workbook = OpenWorkbook(base64);

        Assert.That(workbook.Worksheets, Has.Count.EqualTo(2));
        Assert.That(workbook.Worksheets.Worksheet(1).Name, Is.EqualTo("First"));
        Assert.That(workbook.Worksheets.Worksheet(2).Name, Is.EqualTo("Second"));
    }

    [Test]
    public void ExportToExcel_DataRowsMatchInput()
    {
        var document = CreateDocument(
        [
            CreateTable("Data", 1, [["Val1", "Val2"], ["Val3", "Val4"]])
        ]);

        var base64 = _exporter.ExportToExcel(document);
        using var workbook = OpenWorkbook(base64);

        var ws = workbook.Worksheets.Worksheet(1);

        // Header rows: 1 (title), 2 (company), 3 (blank), 4 (filter), 5 (issue), 6-7 (blank), content header at 7, data starts at 8
        Assert.That(ws.Cell(8, 1).GetString(), Is.EqualTo("Val1"));
        Assert.That(ws.Cell(8, 2).GetString(), Is.EqualTo("Val2"));
        Assert.That(ws.Cell(9, 1).GetString(), Is.EqualTo("Val3"));
        Assert.That(ws.Cell(9, 2).GetString(), Is.EqualTo("Val4"));
    }

    [Test]
    public void ExportToExcel_IncludesHeaderRows()
    {
        var document = CreateDocument(
        [
            CreateTable("Report", 1, [["Data1"]])
        ]);

        var base64 = _exporter.ExportToExcel(document);
        using var workbook = OpenWorkbook(base64);

        var ws = workbook.Worksheets.Worksheet(1);

        // Row 1: Title
        Assert.That(ws.Cell(1, 1).GetString(), Is.EqualTo("Test Title"));
        // Row 2: Company info
        Assert.That(ws.Cell(2, 1).GetString(), Does.Contain("TestCorp"));
        // Row 4: Filter description
        Assert.That(ws.Cell(4, 1).GetString(), Is.EqualTo("All records"));
        // Row 5: Issue user and date
        Assert.That(ws.Cell(5, 1).GetString(), Does.Contain("admin"));
    }

    [Test]
    public void ExportToExcel_ColumnHeaders_MatchLabels()
    {
        var col1 = new Column { Order = 1, Label = "Name", Name = "Name" };
        var col2 = new Column { Order = 2, Label = "Age", Name = "Age" };

        var table = new Table.Models.Table
        {
            Info = new Info
            {
                Name = "Sheet1",
                Order = 1,
                Title = "Test",
                CompanyAbbreviation = "Co",
                CompanyUrl = "co.com",
                FilterDescription = "None",
                IssueUser = "user",
                IssueDate = new DateTime(2026, 1, 1)
            },
            Style = new Style
            {
                BackgroundHeaderColor = "#336699",
                FontHeaderColor = "#FFFFFF"
            },
            Content = new Body
            {
                Header = new Header { Columns = [col1, col2] },
                Rows =
                [
                    new Row
                    {
                        Order = 1,
                        Cells =
                        [
                            new Cell { Column = col1, Value = "Alice" },
                            new Cell { Column = col2, Value = "30" }
                        ]
                    }
                ]
            }
        };

        var document = new Document { Tables = [table] };

        var base64 = _exporter.ExportToExcel(document);
        using var workbook = OpenWorkbook(base64);

        var ws = workbook.Worksheets.Worksheet(1);

        // Content header row is at row 7
        Assert.That(ws.Cell(7, 1).GetString(), Is.EqualTo("Name"));
        Assert.That(ws.Cell(7, 2).GetString(), Is.EqualTo("Age"));
    }

    private static Document CreateDocument(List<Table.Models.Table> tables)
    {
        return new Document { Tables = tables };
    }

    private static Table.Models.Table CreateTable(string name, short order, List<string[]> rowData)
    {
        var columns = new List<Column>();
        short maxCols = rowData.Count > 0 ? (short)rowData.Max(r => r.Length) : (short)0;

        for (short i = 1; i <= maxCols; i++)
            columns.Add(new Column { Order = i, Label = $"Col{i}", Name = $"Col{i}" });

        var rows = new List<Row>();
        short rowOrder = 1;

        foreach (var cellValues in rowData)
        {
            var cells = new List<Cell>();
            short colOrder = 1;

            foreach (var value in cellValues)
            {
                cells.Add(new Cell
                {
                    Column = columns[colOrder - 1],
                    Value = value
                });
                colOrder++;
            }

            rows.Add(new Row { Order = rowOrder, Cells = cells });
            rowOrder++;
        }

        return new Table.Models.Table
        {
            Info = new Info
            {
                Name = name,
                Order = order,
                Title = "Test Title",
                CompanyAbbreviation = "TestCorp",
                CompanyUrl = "testcorp.com",
                FilterDescription = "All records",
                IssueUser = "admin",
                IssueDate = new DateTime(2026, 1, 1)
            },
            Style = new Style
            {
                BackgroundHeaderColor = "#336699",
                FontHeaderColor = "#FFFFFF"
            },
            Content = new Body
            {
                Header = new Header { Columns = columns },
                Rows = rows
            }
        };
    }

    private static XLWorkbook OpenWorkbook(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        var ms = new MemoryStream(bytes);
        return new XLWorkbook(ms);
    }
}
