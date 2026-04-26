using System.Text;
using NUnit.Framework;
using NuvTools.Report.FixedLength;
using NuvTools.Report.FixedLength.Attributes;
using NuvTools.Report.Parsing;
using NuvTools.Report.Parsing.Converters;

namespace NuvTools.Report.FixedLength.Tests;

[TestFixture]
public class FixedLengthReaderTests
{
    private FixedLengthReader _reader = null!;

    [SetUp]
    public void SetUp() => _reader = new FixedLengthReader();

    public class BasicRecord
    {
        [FixedLengthField(0, 3)]
        public string Code { get; set; } = "";

        [FixedLengthField(1, 10, Trim = TrimMode.Both)]
        public string Name { get; set; } = "";

        [FixedLengthField(2, 8)]
        public int Quantity { get; set; }

        [FixedLengthField(3, 8, Format = "yyyyMMdd")]
        public DateTime Date { get; set; }
    }

    [Test]
    public void ReadString_ParsesAllScalarTypes()
    {
        var content = string.Concat(
            "ABC", "Widget    ", "00000010", "20260101", "\n",
            "XYZ", "Gadget    ", "00000025", "20260315");

        var records = _reader.ReadString<BasicRecord>(content);

        Assert.That(records, Has.Count.EqualTo(2));
        Assert.That(records[0].Code, Is.EqualTo("ABC"));
        Assert.That(records[0].Name, Is.EqualTo("Widget"));
        Assert.That(records[0].Quantity, Is.EqualTo(10));
        Assert.That(records[0].Date, Is.EqualTo(new DateTime(2026, 1, 1)));
        Assert.That(records[1].Code, Is.EqualTo("XYZ"));
        Assert.That(records[1].Name, Is.EqualTo("Gadget"));
        Assert.That(records[1].Date, Is.EqualTo(new DateTime(2026, 3, 15)));
    }

    [FixedLengthRecord(AllowShorterLines = true)]
    public class ShortLineRecord
    {
        [FixedLengthField(0, 3)]
        public string Code { get; set; } = "";

        [FixedLengthField(1, 10, Trim = TrimMode.Both)]
        public string? Name { get; set; }
    }

    [Test]
    public void AllowShorterLines_AcceptsTruncatedLastLine()
    {
        var content = "ABCWidget    \nXYZ";

        var records = _reader.ReadString<ShortLineRecord>(content);

        Assert.That(records, Has.Count.EqualTo(2));
        Assert.That(records[1].Code, Is.EqualTo("XYZ"));
        Assert.That(records[1].Name, Is.Null);
    }

    public class OptionalFieldRecord
    {
        [FixedLengthField(0, 3)]
        public string Code { get; set; } = "";

        [FixedLengthField(1, 20, Optional = true, Trim = TrimMode.Right)]
        public string? Description { get; set; }
    }

    [Test]
    public void OptionalField_TolerantToAbsentColumn()
    {
        var content = "ABC\nXYZHello world         ";

        var records = _reader.ReadString<OptionalFieldRecord>(content);

        Assert.That(records, Has.Count.EqualTo(2));
        Assert.That(records[0].Description, Is.Null);
        Assert.That(records[1].Description, Is.EqualTo("Hello world"));
    }

    public class FilteredRecord
    {
        [FixedLengthField(0, 5)]
        public string Value { get; set; } = "";
    }

    [Test]
    public void LineFilter_SkipsHeaderAndFooter()
    {
        var content = "HEAD!\nROW01\nROW02\nFOOT!";

        var records = _reader.ReadString<FilteredRecord>(content, new FixedLengthReaderOptions
        {
            LineFilter = line => line.StartsWith('R')
        });

        Assert.That(records, Has.Count.EqualTo(2));
        Assert.That(records[0].Value, Is.EqualTo("ROW01"));
        Assert.That(records[1].Value, Is.EqualTo("ROW02"));
    }

    public class TrimRecord
    {
        [FixedLengthField(0, 10, Trim = TrimMode.Both)]
        public string Padded { get; set; } = "";
    }

    [Test]
    public void TrimMode_BothStripsLeadingAndTrailingWhitespace()
    {
        var records = _reader.ReadString<TrimRecord>("  hello   ");

        Assert.That(records[0].Padded, Is.EqualTo("hello"));
    }

    public class BrazilianDecimalConverter : FieldConverter<decimal>
    {
        public override decimal Convert(string value, string? format)
        {
            var decimalPlaces = int.Parse(format ?? "2");
            var raw = long.Parse(value);
            return raw / (decimal)Math.Pow(10, decimalPlaces);
        }
    }

    public class FinancialRecord
    {
        [FixedLengthField(0, 9, Converter = typeof(BrazilianDecimalConverter), Format = "2")]
        public decimal Amount { get; set; }
    }

    [Test]
    public void CustomConverter_IsInvoked()
    {
        var records = _reader.ReadString<FinancialRecord>("000000150");

        Assert.That(records[0].Amount, Is.EqualTo(1.50m));
    }

    [Test]
    public void ReadBase64_AndReadStream_MatchReadString()
    {
        var content = "ABCWidget    0000001020260101";
        var bytes = Encoding.UTF8.GetBytes(content);
        var base64 = Convert.ToBase64String(bytes);

        var fromString = _reader.ReadString<BasicRecord>(content);
        var fromBase64 = _reader.ReadBase64<BasicRecord>(base64);
        using var stream = new MemoryStream(bytes);
        var fromStream = _reader.ReadStream<BasicRecord>(stream);

        Assert.That(fromString[0].Code, Is.EqualTo(fromBase64[0].Code));
        Assert.That(fromString[0].Quantity, Is.EqualTo(fromStream[0].Quantity));
        Assert.That(fromString[0].Date, Is.EqualTo(fromBase64[0].Date));
    }
}
