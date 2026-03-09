using NUnit.Framework;
using NuvTools.Report.Csv;
using NuvTools.Report.Sheet.Csv;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Sheet.Tests;

[TestFixture]
public class CsvExporterTests
{
    private CsvExporter _exporter = null!;

    [SetUp]
    public void SetUp()
    {
        _exporter = new CsvExporter();
    }

    [Test]
    public void ExportFirstSheetToCsv_SingleTable_ReturnsCorrectCsv()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A1", "B1"], ["A2", "B2"]])
        ]);

        var base64 = _exporter.ExportFirstSheetToCsv(document);
        var lines = DecodeBase64ToLines(base64);

        Assert.That(lines, Has.Count.EqualTo(2));
        Assert.That(lines[0], Is.EqualTo("A1,B1"));
        Assert.That(lines[1], Is.EqualTo("A2,B2"));
    }

    [Test]
    public void ExportToCsv_MultipleTables_ReturnsOneBase64PerTable()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["X1", "Y1"]]),
            CreateTable("Sheet2", [["X2", "Y2"], ["X3", "Y3"]])
        ]);

        var result = _exporter.ExportToCsv(document);

        Assert.That(result, Has.Count.EqualTo(2));

        var lines1 = DecodeBase64ToLines(result[0]);
        Assert.That(lines1, Has.Count.EqualTo(1));
        Assert.That(lines1[0], Is.EqualTo("X1,Y1"));

        var lines2 = DecodeBase64ToLines(result[1]);
        Assert.That(lines2, Has.Count.EqualTo(2));
        Assert.That(lines2[0], Is.EqualTo("X2,Y2"));
        Assert.That(lines2[1], Is.EqualTo("X3,Y3"));
    }

    [Test]
    public void ExportFirstSheetToCsv_SemicolonDelimiter_UsesSemicolon()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A", "B", "C"]])
        ]);

        var base64 = _exporter.ExportFirstSheetToCsv(document, CsvDelimiter.Semicolon);
        var lines = DecodeBase64ToLines(base64);

        Assert.That(lines[0], Is.EqualTo("A;B;C"));
    }

    [Test]
    public void ExportFirstSheetToCsv_TabDelimiter_UsesTab()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A", "B"]])
        ]);

        var base64 = _exporter.ExportFirstSheetToCsv(document, CsvDelimiter.Tab);
        var lines = DecodeBase64ToLines(base64);

        Assert.That(lines[0], Is.EqualTo("A\tB"));
    }

    [Test]
    public void ExportFirstSheetToCsv_CustomDelimiter_UsesCustomString()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A", "B", "C"]])
        ]);

        var base64 = _exporter.ExportFirstSheetToCsv(document, CsvDelimiter.Custom, "||");
        var lines = DecodeBase64ToLines(base64);

        Assert.That(lines[0], Is.EqualTo("A||B||C"));
    }

    [Test]
    public void ExportToCsv_EmptyDocument_ReturnsEmptyList()
    {
        var document = new Document();

        var result = _exporter.ExportToCsv(document);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ExportToCsv_TableWithNoRows_ReturnsEmptyBase64()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [])
        ]);

        var result = _exporter.ExportToCsv(document);

        Assert.That(result, Has.Count.EqualTo(1));
        var lines = DecodeBase64ToLines(result[0]);
        Assert.That(lines, Is.Empty);
    }

    [Test]
    public void ExportFirstSheetToCsv_CellsOrderedByColumnOrder()
    {
        var col1 = new Column { Order = 2, Label = "Col2" };
        var col2 = new Column { Order = 1, Label = "Col1" };

        var table = new Table.Models.Table
        {
            Info = new Info { Name = "Sheet1", Order = 1 },
            Content = new Body
            {
                Rows =
                [
                    new Row
                    {
                        Order = 1,
                        Cells =
                        [
                            new Cell { Column = col1, Value = "Second" },
                            new Cell { Column = col2, Value = "First" }
                        ]
                    }
                ]
            }
        };

        var document = new Document { Tables = [table] };

        var base64 = _exporter.ExportFirstSheetToCsv(document);
        var lines = DecodeBase64ToLines(base64);

        Assert.That(lines[0], Is.EqualTo("First,Second"));
    }

    private static Document CreateDocument(List<Table.Models.Table> tables)
    {
        return new Document { Tables = tables };
    }

    private static Table.Models.Table CreateTable(string name, List<string[]> rowData)
    {
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
                    Column = new Column { Order = colOrder },
                    Value = value
                });
                colOrder++;
            }

            rows.Add(new Row { Order = rowOrder, Cells = cells });
            rowOrder++;
        }

        return new Table.Models.Table
        {
            Info = new Info { Name = name, Order = 1 },
            Content = new Body { Rows = rows }
        };
    }

    private static List<string> DecodeBase64ToLines(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        using var ms = new MemoryStream(bytes);
        using var sr = new StreamReader(ms);

        var lines = new List<string>();
        while (sr.ReadLine() is { } line)
            lines.Add(line);

        return lines;
    }
}
