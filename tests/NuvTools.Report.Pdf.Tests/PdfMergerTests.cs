using System.Runtime.InteropServices;
using NUnit.Framework;
using NuvTools.Report.Pdf.Table;
using NuvTools.Report.Pdf.Util;
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Pdf.Tests;

[TestFixture]
public class PdfMergerTests
{
    private PdfMerger _merger = null!;
    private PdfExporter _exporter = null!;

    [SetUp]
    public void SetUp()
    {
        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64 &&
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.Ignore("QuestPDF does not support win-arm64.");

        _merger = new PdfMerger();
        _exporter = new PdfExporter();
    }

    [Test]
    public void Merge_TwoPdfs_ReturnsValidPdfBytes()
    {
        var pdf1 = GeneratePdfBytes("Sheet1", [["A1", "B1"]]);
        var pdf2 = GeneratePdfBytes("Sheet2", [["C1", "D1"]]);

        var result = _merger.Merge([pdf1, pdf2]);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result[0], Is.EqualTo((byte)'%'));
        Assert.That(result[1], Is.EqualTo((byte)'P'));
        Assert.That(result[2], Is.EqualTo((byte)'D'));
        Assert.That(result[3], Is.EqualTo((byte)'F'));
    }

    [Test]
    public void Merge_EmptyCollection_ReturnsValidPdfBytes()
    {
        var result = _merger.Merge([]);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result[0], Is.EqualTo((byte)'%'));
    }

    private byte[] GeneratePdfBytes(string name, List<string[]> rowData)
    {
        var document = CreateDocument(name, rowData);
        var base64 = _exporter.ExportFirstSheetToPdf(document);
        return Convert.FromBase64String(base64);
    }

    private static Document CreateDocument(string name, List<string[]> rowData)
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

        var table = new Report.Table.Models.Table
        {
            Info = new Info { Name = name, Order = 1 },
            Content = new Body
            {
                Header = new Header { Columns = columns },
                Rows = rows
            }
        };

        return new Document { Tables = [table] };
    }
}
