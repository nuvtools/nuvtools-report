# NuvTools.Report

[![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/)
[![License](https://img.shields.io/github/license/nuvtools/nuvtools-report.svg)](LICENSE)

A .NET library suite for generating reports in PDF, Excel, and CSV formats, plus reading/importing CSV and fixed-length (positional) files. Build structured table-based reports with styling, company branding, and reflection-based data binding. Targets .NET 8, .NET 9, and .NET 10.

## Libraries

| Library | Description | NuGet |
|---------|-------------|-------|
| **NuvTools.Report** | Core report model, PDF abstractions, Sheet abstractions (CSV, Excel), FixedLength abstractions, and shared parsing utilities — with attribute-based field mapping and custom converters | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/) |
| **NuvTools.Report.Pdf** | PDF generation implementation using QuestPDF and PDFsharp | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Pdf.svg)](https://www.nuget.org/packages/NuvTools.Report.Pdf/) |
| **NuvTools.Report.Sheet** | Excel and CSV implementation using ClosedXML | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Sheet.svg)](https://www.nuget.org/packages/NuvTools.Report.Sheet/) |
| **NuvTools.Report.FixedLength** | Fixed-length (positional) file reader implementation | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.FixedLength.svg)](https://www.nuget.org/packages/NuvTools.Report.FixedLength/) |

## Installation

```bash
# For PDF export
dotnet add package NuvTools.Report.Pdf

# For Excel and CSV export/reading
dotnet add package NuvTools.Report.Sheet

# For fixed-length (positional) file reading
dotnet add package NuvTools.Report.FixedLength

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
| `NuvTools.Report.FixedLength` | `NuvTools.Report.FixedLength` |
| `NuvTools.Report.Parsing` | Shared parsing utilities and converters (consumed by Csv and FixedLength readers) |

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

builder.Services.AddSheetReportServices();       // ICsvReader, ICsvExporter, IExcelExporter
builder.Services.AddFixedLengthReportServices(); // IFixedLengthReader
builder.Services.AddPdfReportServices();         // IPdfExporter, IPdfMerger
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

The output is UTF-8 with a byte order mark, so Excel opens non-ASCII text correctly. Date values are written with `Column.Format` when one is set; give date columns an explicit format (e.g. `yyyy-MM-dd HH:mm:ss`) so the output does not depend on the server culture.

### CSV Reading / Importing

Map CSV content to strongly-typed objects using attributes:

```csharp
using NuvTools.Report.Sheet.Csv;
using NuvTools.Report.Sheet.Csv.Attributes;
using NuvTools.Report.Parsing;

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
using NuvTools.Report.FixedLength;
using NuvTools.Report.FixedLength.Attributes;
using NuvTools.Report.Parsing;

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
using NuvTools.Report.Parsing.Converters;

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

### NuvTools.Report.FixedLength
- No external runtime dependencies (only the base `NuvTools.Report` package)

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
│   ├── NuvTools.Report/                # Core models and abstractions
│   │   ├── Pdf/                        # IPdfExporter, IPdfMerger
│   │   ├── Sheet/
│   │   │   ├── Csv/                    # ICsvReader, ICsvExporter, attributes, options
│   │   │   └── Excel/                  # IExcelExporter
│   │   ├── FixedLength/                # IFixedLengthReader, attributes, options
│   │   ├── Parsing/                    # TrimMode, ParseException, converters (shared)
│   │   └── Table/Models/               # Document, Table, Info, Style, Body, Row, Cell, Column
│   ├── NuvTools.Report.Pdf/            # PDF implementation (QuestPDF + PDFsharp)
│   ├── NuvTools.Report.Sheet/          # Sheet implementation (ClosedXML)
│   └── NuvTools.Report.FixedLength/    # Fixed-length reader implementation
├── tests/
│   ├── NuvTools.Report.Pdf.Tests/
│   ├── NuvTools.Report.Sheet.Tests/
│   └── NuvTools.Report.FixedLength.Tests/
├── NuvTools.Report.slnx
└── README.md
```

## Releasing

Publishing to nuget.org is irreversible per version, so the workflow makes it the last and smallest
step.

**One workflow per library.** Each package has its own, so a run and an approval each concern
exactly one package. The steps themselves are shared, in the reusable `package-publish.yml`.

| Package | Workflow | Release tag |
|---|---|---|
| `NuvTools.Report` | `report-publish.yml` | `report-vX.Y.Z` |
| `NuvTools.Report.FixedLength` | `fixedlength-publish.yml` | `fixedlength-vX.Y.Z` |
| `NuvTools.Report.Pdf` | `pdf-publish.yml` | `pdf-vX.Y.Z` |
| `NuvTools.Report.Sheet` | `sheet-publish.yml` | `sheet-vX.Y.Z` |

1. Set the package's `<Version>` in its `.csproj` and merge it. That is the version that gets
   published; the workflow reads it and never computes one.
2. Run the library's workflow by hand (*Actions → Report Publish → Run workflow*) with *push*
   checked.
3. The workflow builds the solution with `-warnaserror`, runs the tests, takes **that package only**
   from the build output, verifies it carries its README, licence, icon, symbols and every target
   framework, and then waits for approval on the `nuget` GitHub Environment before pushing.
4. **After the push succeeds, the workflow tags the commit** with the tag in the table above. The tag is a
   record of what is on nuget.org, never what starts a release, so it cannot exist for a version
   that did not get there. A run whose tag already exists stops before building: the version has
   been released and `<Version>` needs a bump.

**A referenced package goes first.** A package that references another of this repository as a
project depends on whatever `<Version>` that one has in the commit. The workflow checks that
version is on nuget.org and refuses to push a package whose dependency is not there yet. So:

- `NuvTools.Report` before `NuvTools.Report.FixedLength`, `NuvTools.Report.Pdf` and `NuvTools.Report.Sheet`.

That check knows a version exists, not what is in it: when a referenced package changes its public
API, bump and publish it first, then bump and publish the packages built on it.

**Versions published by hand are refused.** Releases made before these workflows exist on nuget.org
with no tag. Pushing one again would skip the duplicate and then tag the current commit as a
release it is not, so a version that is already on nuget.org stops the run. The exception is a
re-run of the same run, which is how a push that succeeded and a tag that failed gets finished.

**No API key is stored.** The push uses nuget.org's trusted publishing: the job presents a GitHub
OIDC token, nuget.org checks it against the policy registered for this repository and answers with a
key that lives for one hour. The repository needs one secret, `NUGET_USER` — the nuget.org profile
name the policy belongs to — and the policy itself, registered under *Trusted Publishing* on
nuget.org with the owner `nuvtools`, the repository `nuvtools-report`, the workflow file and the
environment `nuget`.

Run a library's workflow by hand with *push* unchecked for a dry run that stops after the
verification.

## License

This project requires license acceptance. See the [LICENSE](LICENSE) file for details.

## Links

- [Website](https://nuvtools.com)
- [GitHub Repository](https://github.com/nuvtools/nuvtools-report)
- [NuGet Packages](https://www.nuget.org/profiles/NuvTools)
