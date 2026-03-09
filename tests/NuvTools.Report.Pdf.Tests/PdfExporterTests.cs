using System.Runtime.InteropServices;
using NUnit.Framework;
using NuvTools.Report.Pdf.Table;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Pdf.Tests;

[TestFixture]
public class PdfExporterTests
{
    private PdfExporter _exporter = null!;

    [SetUp]
    public void SetUp()
    {
        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64 &&
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.Ignore("QuestPDF does not support win-arm64.");

        _exporter = new PdfExporter();
    }

    [Test]
    public void ExportFirstSheetToPdf_ReturnsValidBase64Pdf()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A1", "B1"], ["A2", "B2"]])
        ]);

        var base64 = _exporter.ExportFirstSheetToPdf(document);

        Assert.That(base64, Is.Not.Null.And.Not.Empty);
        var bytes = Convert.FromBase64String(base64);
        Assert.That(bytes[0], Is.EqualTo((byte)'%'));
        Assert.That(bytes[1], Is.EqualTo((byte)'P'));
        Assert.That(bytes[2], Is.EqualTo((byte)'D'));
        Assert.That(bytes[3], Is.EqualTo((byte)'F'));
    }

    [Test]
    public void ExportSheetToPdf_SingleTable_ReturnsOneEntry()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A1", "B1"]])
        ]);

        var result = _exporter.ExportSheetToPdf(document);

        Assert.That(result, Has.Count.EqualTo(1));
        var bytes = Convert.FromBase64String(result[0]);
        Assert.That(bytes[0], Is.EqualTo((byte)'%'));
        Assert.That(bytes[1], Is.EqualTo((byte)'P'));
        Assert.That(bytes[2], Is.EqualTo((byte)'D'));
        Assert.That(bytes[3], Is.EqualTo((byte)'F'));
    }

    [Test]
    public void ExportSheetToPdf_MultipleTables_ReturnsOneEntryPerTable()
    {
        var document = CreateDocument(
        [
            CreateTable("Sheet1", [["A1", "B1"]]),
            CreateTable("Sheet2", [["C1", "D1"]]),
            CreateTable("Sheet3", [["E1", "F1"]])
        ]);

        var result = _exporter.ExportSheetToPdf(document);

        Assert.That(result, Has.Count.EqualTo(3));
        foreach (var base64 in result)
        {
            var bytes = Convert.FromBase64String(base64);
            Assert.That(bytes[0], Is.EqualTo((byte)'%'));
        }
    }

    private static Document CreateDocument(List<Report.Table.Models.Table> tables)
    {
        return new Document { Tables = tables };
    }

    private static Report.Table.Models.Table CreateTable(string name, List<string[]> rowData)
    {
        var columns = new List<Column>();
        if (rowData.Count > 0)
        {
            for (short i = 1; i <= rowData[0].Length; i++)
                columns.Add(new Column { Order = i, Label = $"Col{i}", Name = $"Col{i}" });
        }

        var rows = new List<Row>();
        short rowOrder = 1;

        foreach (var cellValues in rowData)
        {
            var cells = new List<Cell>();
            short colOrder = 0;

            foreach (var value in cellValues)
            {
                cells.Add(new Cell
                {
                    Column = columns[colOrder],
                    Value = value
                });
                colOrder++;
            }

            rows.Add(new Row { Order = rowOrder, Cells = cells });
            rowOrder++;
        }

        return new Report.Table.Models.Table
        {
            Info = new Info { Name = name, Order = 1 },
            Content = new Body
            {
                Header = new Header { Columns = columns },
                Rows = rows
            }
        };
    }
}
