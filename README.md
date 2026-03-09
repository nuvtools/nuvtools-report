# NuvTools.Report

[![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/)
[![License](https://img.shields.io/github/license/nuvtools/nuvtools-report.svg)](LICENSE)

A .NET library suite for generating reports in PDF, Excel, and CSV formats, plus reading/importing CSV and fixed-length (positional) files. Build structured table-based reports with styling, company branding, and reflection-based data binding. Targets .NET 8, .NET 9, and .NET 10.

## Libraries

| Library | Description | NuGet |
|---------|-------------|-------|
| **NuvTools.Report** | Core report model, PDF abstractions, and Sheet abstractions (CSV, Excel, fixed-length) with attribute-based field mapping and custom converters | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/) |
| **NuvTools.Report.Pdf** | PDF generation implementation using QuestPDF and PDFsharp | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Pdf.svg)](https://www.nuget.org/packages/NuvTools.Report.Pdf/) |
| **NuvTools.Report.Sheet** | Excel, CSV, and fixed-length file implementation using ClosedXML | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Sheet.svg)](https://www.nuget.org/packages/NuvTools.Report.Sheet/) |

## Installation

```bash
# For PDF export
dotnet add package NuvTools.Report.Pdf

# For Excel/CSV export and CSV/fixed-length reading
dotnet add package NuvTools.Report.Sheet

# Or install the base library only (abstractions and models)
dotnet add package NuvTools.Report
```

## Features

- **Export**: PDF (landscape A4), Excel (.xlsx), CSV with configurable delimiters
- **Import**: CSV and fixed-length (positional) file reading with attribute-based mapping
- **Data binding**: Automatic population from object collections via reflection
- **Custom converters**: Extensible field conversion with `IFieldConverter` / `FieldConverter<T>`
- **Styling**: Customizable colors, fonts, and formatting
- **Branding**: Company logos, names, and URLs in headers
- **Metadata**: Titles, filter descriptions, issue dates, user info
- **Output**: Base64-encoded output for API transmission
- **Multi-table**: Documents with separate worksheets/pages
- **PDF merging**: Combine multiple PDFs into one
- **DI integration**: `AddReportServices()` and `AddPdfReportServices()` extensions

## Architecture

### Namespace Structure

The library follows an **abstraction-implementation** pattern:

| Abstraction (NuvTools.Report) | Implementation |
|-------------------------------|----------------|
| `NuvTools.Report.Pdf` | `NuvTools.Report.Pdf.*` (QuestPDF + PDFsharp) |
| `NuvTools.Report.Sheet.Csv` | `NuvTools.Report.Sheet.Csv` (ClosedXML) |
| `NuvTools.Report.Sheet.Excel` | `NuvTools.Report.Sheet.Excel` (ClosedXML) |
| `NuvTools.Report.Sheet.FixedLength` | `NuvTools.Report.Sheet.FixedLength` |
| `NuvTools.Report.Sheet.Parsing` | Shared parsing utilities and converters |

### Document Model

The library uses a document-table-component hierarchy:

```
Document
└── Tables (List<Table>)
    ├── Info (metadata: name, title, company info, issue date/user)
    ├── Style (formatting: colors, fonts)
    └── Content (Body)
        ├── Header
        │   └── Columns (List<Column>)
        └── Rows (List<Row>)
            └── Cells (List<Cell>)
```

## Usage

### Dependency Injection Setup

```csharp
using NuvTools.Report.Sheet.Extensions;
using NuvTools.Report.Pdf.Extensions;

builder.Services.AddSheetReportServices();    // ICsvReader, ICsvExporter, IFixedLengthReader, IExcelExporter
builder.Services.AddPdfReportServices(); // IPdfExporter, IPdfMerger
```

### Building a Document

```csharp
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;

// Define columns
var columns = new List<Column>
{
    new() { Name = "Id", Label = "ID", Order = 1, Format = "" },
    new() { Name = "Name", Label = "Product Name", Order = 2, Format = "" },
    new() { Name = "Price", Label = "Price", Order = 3, Format = "C2" },
    new() { Name = "Date", Label = "Date", Order = 4, Format = "yyyy-MM-dd" }
};

// Create a table
var table = new Table
{
    Info = new Info
    {
        Name = "Products",
        Title = "Product Catalog",
        CompanyAbbreviation = "ACME Corp",
        CompanyUrl = "https://acme.com",
        FilterDescription = "All Products",
        IssueDate = DateTime.Now,
        IssueUser = "john.doe"
    },
    Style = new Style
    {
        BackgroundHeaderColor = "#003366",
        FontHeaderColor = "#FFFFFF"
    },
    Content = new Body()
};

// Populate rows from objects using reflection
table.SetRows(columns, products);

// Create document
var document = new Document
{
    BackgroundDocumentHeaderColor = "#003366",
    Tables = [table]
};
```

### PDF Export

```csharp
using NuvTools.Report.Pdf;

// Via DI
public class ReportService(IPdfExporter pdfExporter, IPdfMerger pdfMerger)
{
    public string GeneratePdf(Document document)
    {
        // Export first table as base64 PDF
        return pdfExporter.ExportFirstSheetToPdf(document);
    }

    public List<string> GenerateAllPdfs(Document document)
    {
        // Export all tables as separate base64 PDFs
        return pdfExporter.ExportSheetToPdf(document);
    }

    public byte[] MergePdfs(IEnumerable<byte[]> pdfs)
    {
        return pdfMerger.Merge(pdfs);
    }
}
```

### Excel Export

```csharp
using NuvTools.Report.Sheet.Excel;

// Via DI
public class ReportService(IExcelExporter excelExporter)
{
    public string GenerateExcel(Document document)
    {
        // Returns base64-encoded .xlsx with styled headers, column definitions, and data rows
        string base64 = excelExporter.ExportToExcel(document);

        // Save to file
        byte[] bytes = Convert.FromBase64String(base64);
        File.WriteAllBytes("report.xlsx", bytes);

        return base64;
    }
}
```

### CSV Export

```csharp
using NuvTools.Report.Sheet.Csv;

// Via DI
public class ReportService(ICsvExporter csvExporter)
{
    public void ExportCsv(Document document)
    {
        // Default delimiter is comma (,) per RFC 4180
        // Header row (column labels) is included by default
        List<string> csvFiles = csvExporter.ExportToCsv(document);
        string csvBase64 = csvExporter.ExportFirstSheetToCsv(document);

        // Use a different delimiter
        string csvSemicolon = csvExporter.ExportFirstSheetToCsv(document, CsvDelimiter.Semicolon);
        string csvTab = csvExporter.ExportFirstSheetToCsv(document, CsvDelimiter.Tab);

        // Use a custom delimiter
        string csvPipe = csvExporter.ExportFirstSheetToCsv(document, CsvDelimiter.Custom, "||");

        // Export without the header row
        string csvNoHeader = csvExporter.ExportFirstSheetToCsv(document, includeHeader: false);

        // By default, delimiter occurrences in values/headers are removed to prevent CSV corruption
        // Disable sanitization if you want to keep raw values
        string csvRaw = csvExporter.ExportFirstSheetToCsv(document, sanitizeDelimiter: false);
    }
}
```

### CSV Reading / Importing

Map CSV content to strongly-typed objects using attributes:

```csharp
using NuvTools.Report.Sheet.Csv;
using NuvTools.Report.Sheet.Csv.Attributes;
using NuvTools.Report.Sheet.Parsing;

// Define your record model
[CsvRecord(Delimiter = CsvDelimiter.Semicolon)]
public class ProductRecord
{
    [CsvField(0, Caption = "ID")]
    public int Id { get; set; }

    [CsvField(1, Caption = "Name")]
    public string Name { get; set; } = "";

    [CsvField(2, Caption = "Price", Trim = TrimMode.Both)]
    public decimal Price { get; set; }

    [CsvField(3, Caption = "Date", Format = "yyyy-MM-dd")]
    public DateTime Date { get; set; }

    [CsvField(4)]
    public string? OptionalNote { get; set; }
}

// Read CSV via DI
public class ImportService(ICsvReader csvReader)
{
    public List<ProductRecord> ImportFromString(string csvContent)
    {
        return csvReader.ReadString<ProductRecord>(csvContent);
    }

    public List<ProductRecord> ImportFromBase64(string base64)
    {
        return csvReader.ReadBase64<ProductRecord>(base64);
    }

    public List<ProductRecord> ImportFromStream(Stream stream)
    {
        return csvReader.ReadStream<ProductRecord>(stream, new CsvReaderOptions
        {
            SkipHeader = true,
            IgnoreEmptyLines = true,
            HandleQuotedFields = true
        });
    }

    public List<ProductRecord> ImportWithDelimiterOverride(string csvContent)
    {
        // Override the attribute-level delimiter at runtime
        return csvReader.ReadString<ProductRecord>(csvContent, new CsvReaderOptions
        {
            Delimiter = CsvDelimiter.Comma
        });
    }
}
```

#### Generating CSV Headers

```csharp
using NuvTools.Report.Sheet.Csv;

// Get ordered captions from CsvField attributes
IEnumerable<string> captions = typeof(ProductRecord).GetFieldCaptions();
// ["ID", "Name", "Price", "Date", "OptionalNote"]

// Get a formatted header line
string header = typeof(ProductRecord).GetCsvHeader(CsvDelimiter.Semicolon);
// "ID;Name;Price;Date;OptionalNote"
```

### Fixed-Length (Positional) File Reading

Map fixed-width columns to objects using attributes:

```csharp
using NuvTools.Report.Sheet.FixedLength;
using NuvTools.Report.Sheet.FixedLength.Attributes;
using NuvTools.Report.Sheet.Parsing;

// Define your record model
[FixedLengthRecord(AllowShorterLines = true)]
public class BankRecord
{
    [FixedLengthField(0, 3)]                          // positions 0-2 (3 chars)
    public string BankCode { get; set; } = "";

    [FixedLengthField(1, 5, Trim = TrimMode.Both)]    // positions 3-7 (5 chars)
    public string BranchCode { get; set; } = "";

    [FixedLengthField(2, 15, Trim = TrimMode.Both)]   // positions 8-22 (15 chars)
    public decimal Amount { get; set; }

    [FixedLengthField(3, 8, Format = "yyyyMMdd")]      // positions 23-30 (8 chars)
    public DateTime TransactionDate { get; set; }

    [FixedLengthField(4, 30, Optional = true, Trim = TrimMode.Right)] // positions 31-60 (optional)
    public string? Description { get; set; }
}

// Read via DI
public class BankImportService(IFixedLengthReader reader)
{
    public List<BankRecord> Import(string content)
    {
        return reader.ReadString<BankRecord>(content);
    }

    public List<BankRecord> ImportWithFilter(Stream stream)
    {
        return reader.ReadStream<BankRecord>(stream, new FixedLengthReaderOptions
        {
            IgnoreEmptyLines = true,
            LineFilter = line => line.StartsWith("D") // only detail lines
        });
    }

    public List<BankRecord> ImportFromBase64(string base64)
    {
        return reader.ReadBase64<BankRecord>(base64);
    }
}
```

### Custom Field Converters

Create custom converters for special parsing logic:

```csharp
using NuvTools.Report.Sheet.Parsing.Converters;

// Implement IFieldConverter directly
public class BrazilianDecimalConverter : IFieldConverter
{
    public object? Convert(string value, string? format)
    {
        // "000000150" with format "2" → 1.50
        var decimalPlaces = int.Parse(format ?? "2");
        var raw = long.Parse(value);
        return raw / (decimal)Math.Pow(10, decimalPlaces);
    }
}

// Or use the generic base class for type safety
public class PercentageConverter : FieldConverter<decimal>
{
    public override decimal Convert(string value, string? format)
    {
        return decimal.Parse(value) / 100m;
    }
}

// Use in attributes
public class FinancialRecord
{
    [CsvField(0)]
    public string Code { get; set; } = "";

    [CsvField(1, Converter = typeof(BrazilianDecimalConverter), Format = "2")]
    public decimal Amount { get; set; }

    [FixedLengthField(0, 10, Converter = typeof(PercentageConverter))]
    public decimal Rate { get; set; }
}
```

### Built-in Type Conversions

The following types are supported out of the box (no custom converter needed):

| Type | Notes |
|------|-------|
| `string` | Passed through as-is |
| `int`, `long`, `short` | Parsed with `InvariantCulture` |
| `decimal`, `double`, `float` | Parsed with `InvariantCulture` |
| `bool` | Supports `true`/`false` and `1`/`0` |
| `DateTime` | Supports optional format string (e.g. `"yyyyMMdd"`) |
| `DateOnly` | Supports optional format string |
| `Guid` | Standard GUID parsing |
| Enums | Case-insensitive `Enum.Parse` |
| Nullable versions | All above types supported as `T?` (empty/whitespace → `null`) |

## Dependencies

### NuvTools.Report.Pdf
- QuestPDF
- PDFsharp

### NuvTools.Report.Sheet
- ClosedXML

## Building

This solution uses the `.slnx` (XML-based solution) format.

```bash
dotnet build NuvTools.Report.slnx
dotnet build NuvTools.Report.slnx -c Release
dotnet test NuvTools.Report.slnx
```

## Project Structure

```
nuvtools-report/
├── src/
│   ├── NuvTools.Report/            # Core models and abstractions
│   │   ├── Pdf/                    # IPdfExporter, IPdfMerger
│   │   ├── Sheet/
│   │   │   ├── Csv/               # ICsvReader, ICsvExporter, attributes, options
│   │   │   ├── Excel/             # IExcelExporter
│   │   │   ├── FixedLength/       # IFixedLengthReader, attributes, options
│   │   │   └── Parsing/           # TrimMode, ParseException, converters
│   │   └── Table/Models/          # Document, Table, Info, Style, Body, Row, Cell, Column
│   ├── NuvTools.Report.Pdf/        # PDF implementation (QuestPDF + PDFsharp)
│   └── NuvTools.Report.Sheet/      # Sheet implementation (ClosedXML)
├── tests/
│   ├── NuvTools.Report.Pdf.Tests/
│   └── NuvTools.Report.Sheet.Tests/
├── NuvTools.Report.slnx
└── README.md
```

## License

This project requires license acceptance. See the [LICENSE](LICENSE) file for details.

## Links

- [Website](https://nuvtools.com)
- [GitHub Repository](https://github.com/nuvtools/nuvtools-report)
- [NuGet Packages](https://www.nuget.org/profiles/NuvTools)
