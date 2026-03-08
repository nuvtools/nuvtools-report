# NuvTools.Report

[![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/)
[![License](https://img.shields.io/github/license/nuvtools/nuvtools-report.svg)](LICENSE)

A .NET library suite for generating reports in PDF, Excel, and CSV formats. Build structured table-based reports with styling, company branding, and reflection-based data binding. Targets .NET 8, .NET 9, and .NET 10.

## Libraries

| Library | Description | NuGet |
|---------|-------------|-------|
| **NuvTools.Report** | Core report model infrastructure with document-table hierarchy | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/) |
| **NuvTools.Report.Pdf** | PDF generation using QuestPDF and PDFsharp | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Pdf.svg)](https://www.nuget.org/packages/NuvTools.Report.Pdf/) |
| **NuvTools.Report.Sheet** | Excel and CSV generation using ClosedXML | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Sheet.svg)](https://www.nuget.org/packages/NuvTools.Report.Sheet/) |

## Installation

```bash
# For PDF export
dotnet add package NuvTools.Report.Pdf

# For Excel/CSV export
dotnet add package NuvTools.Report.Sheet

# Or install the base library only
dotnet add package NuvTools.Report
```

## Features

- Multiple export formats: PDF (landscape A4), Excel (.xlsx), CSV
- Automatic data binding from object collections via reflection
- Customizable colors, fonts, and formatting
- Company branding with logos, names, and URLs in headers
- Metadata support: titles, filter descriptions, issue dates, user info
- Base64-encoded output for API transmission
- Multi-table documents with separate worksheets/pages
- DateTime formatting with custom format strings
- PDF merging utility

## Architecture

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

### PDF Export

```csharp
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;
using NuvTools.Report.Pdf.Table;

// Define columns
var columns = new List<Column>
{
    new Column { Name = "Id", Label = "ID", Order = 1, Format = "" },
    new Column { Name = "Name", Label = "Product Name", Order = 2, Format = "" },
    new Column { Name = "Price", Label = "Price", Order = 3, Format = "C2" },
    new Column { Name = "Date", Label = "Date", Order = 4, Format = "yyyy-MM-dd" }
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

// Create document and export
var document = new Document
{
    BackgroundDocumentHeaderColor = "#003366",
    Tables = [table]
};

string pdfBase64 = document.ExportFirstSheetToPdf();
List<string> allPdfs = document.ExportSheetToPdf();
```

### Excel Export

```csharp
using NuvTools.Report.Sheet.Extensions;

string excelBase64 = document.ExportToExcel();

byte[] excelBytes = Convert.FromBase64String(excelBase64);
File.WriteAllBytes("report.xlsx", excelBytes);
```

### CSV Export

```csharp
using NuvTools.Report.Sheet.Extensions;

// Default delimiter is comma (,) per RFC 4180
List<string> csvFiles = document.ExportToCsv();
string csvBase64 = document.ExportFirstSheetToCsv();

// Use a different delimiter
string csvSemicolon = document.ExportFirstSheetToCsv(CsvDelimiter.Semicolon);
string csvTab = document.ExportFirstSheetToCsv(CsvDelimiter.Tab);

// Use a custom delimiter
string csvPipe = document.ExportFirstSheetToCsv(CsvDelimiter.Custom, customDelimiter: "|");
```

### PDF Merging

```csharp
using NuvTools.Report.Pdf.Util;

byte[] mergedPdf = PdfUtil.Merge([pdf1Bytes, pdf2Bytes]);
```

## Dependencies

### NuvTools.Report.Pdf
- QuestPDF [2025.12.3,2026.1.0)
- PDFsharp [6.2.4,6.3.0)

### NuvTools.Report.Sheet
- ClosedXML [0.105.0,0.106.0)

## Building

This solution uses the `.slnx` (XML-based solution) format.

```bash
dotnet build NuvTools.Report.slnx
dotnet build NuvTools.Report.slnx -c Release
```

## Project Structure

```
nuvtools-report/
├── src/
│   ├── NuvTools.Report/
│   ├── NuvTools.Report.Pdf/
│   └── NuvTools.Report.Sheet/
├── NuvTools.Report.slnx
└── README.md
```

## License

This project requires license acceptance. See the [LICENSE](LICENSE) file for details.

## Links

- [Website](https://nuvtools.com)
- [GitHub Repository](https://github.com/nuvtools/nuvtools-report)
- [NuGet Packages](https://www.nuget.org/profiles/NuvTools)
