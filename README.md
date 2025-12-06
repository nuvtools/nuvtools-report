# NuvTools.Report

A comprehensive .NET library suite for generating professional reports in multiple formats including PDF, Excel, and CSV. Build structured table-based reports with rich styling, company branding, and flexible data binding using reflection.

## 📦 Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **NuvTools.Report** | Core library providing the base table model infrastructure | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.svg)](https://www.nuget.org/packages/NuvTools.Report/) |
| **NuvTools.Report.Pdf** | PDF generation using QuestPDF and PDFsharp | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Pdf.svg)](https://www.nuget.org/packages/NuvTools.Report.Pdf/) |
| **NuvTools.Report.Sheet** | Excel and CSV generation using ClosedXML | [![NuGet](https://img.shields.io/nuget/v/NuvTools.Report.Sheet.svg)](https://www.nuget.org/packages/NuvTools.Report.Sheet/) |

## ✨ Features

- **Multiple Export Formats**: Generate reports as PDF (landscape A4), Excel (.xlsx), or CSV
- **Automatic Data Binding**: Use reflection to populate tables from object collections
- **Rich Styling**: Customize colors, fonts, and formatting
- **Company Branding**: Include company logos, names, and URLs in report headers
- **Metadata Support**: Add titles, filter descriptions, issue dates, and user information
- **Base64 Output**: All exports return base64-encoded strings for easy API transmission
- **Multi-Table Documents**: Support for multiple tables in a single document
- **DateTime Formatting**: Automatic formatting of date values using custom format strings
- **PDF Merging**: Utility to merge multiple PDFs into one document

## 🎯 Target Frameworks

- .NET 8.0
- .NET 9.0
- .NET 10.0

## 🚀 Installation

Install the packages via NuGet Package Manager:

```bash
# For PDF export
dotnet add package NuvTools.Report.Pdf

# For Excel/CSV export
dotnet add package NuvTools.Report.Sheet

# Or install the base library only
dotnet add package NuvTools.Report
```

## 📖 Usage

### Basic Example - PDF Export

```csharp
using NuvTools.Report.Table.Models;
using NuvTools.Report.Table.Models.Components;
using NuvTools.Report.Pdf.Table;

// Define your data model
var products = new List<Product>
{
    new Product { Id = 1, Name = "Product A", Price = 29.99m, Date = DateTime.Now },
    new Product { Id = 2, Name = "Product B", Price = 49.99m, Date = DateTime.Now }
};

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

// Populate rows using reflection
table.SetRows(columns, products);

// Create document
var document = new Document
{
    BackgroundDocumentHeaderColor = "#003366",
    Tables = [table]
};

// Export to PDF (returns base64 string)
string pdfBase64 = document.ExportFirstSheetToPdf();

// Or export all tables
List<string> allPdfs = document.ExportSheetToPdf();
```

### Excel Export

```csharp
using NuvTools.Report.Sheet.Extensions;

// Using the same document structure from above
string excelBase64 = document.ExportToExcel();

// Save to file
byte[] excelBytes = Convert.FromBase64String(excelBase64);
File.WriteAllBytes("report.xlsx", excelBytes);
```

### CSV Export

```csharp
using NuvTools.Report.Sheet.Extensions;

// Export all tables to CSV
List<string> csvFiles = document.ExportToCsv();

// Or export first table only
string csvBase64 = document.ExportFirstSheetToCsv();
```

### PDF Merging

```csharp
using NuvTools.Report.Pdf.Util;

var pdf1 = Convert.FromBase64String(pdfBase64String1);
var pdf2 = Convert.FromBase64String(pdfBase64String2);

byte[] mergedPdf = PdfUtil.Merge([pdf1, pdf2]);
```

## 🏗️ Architecture

The library uses a hierarchical document-table-component structure:

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

### Key Components

- **Document**: Top-level container with background color and table list
- **Table**: Individual table with Info, Style, and Content
- **Column**: Column definition with property name (for reflection), label, order, and format
- **Row/Cell**: Data rows containing cells with values
- **Info**: Metadata including title, company details, filter description, issue info
- **Style**: Formatting properties (colors, fonts, etc.)

## 🔧 Dependencies

### NuvTools.Report.Pdf
- QuestPDF (2025.7.4)
- PDFsharp (6.2.3)

### NuvTools.Report.Sheet
- ClosedXML (0.105.0)

## 📝 License

This project requires license acceptance. See the [LICENSE](LICENSE) file for details.

## 🌐 Links

- **Website**: [https://nuvtools.com](https://nuvtools.com)
- **Repository**: [https://github.com/nuvtools/nuvtools-report](https://github.com/nuvtools/nuvtools-report)
- **NuGet Packages**: Search for "NuvTools.Report" on [nuget.org](https://www.nuget.org/)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

---

Copyright © 2025 Nuv Tools