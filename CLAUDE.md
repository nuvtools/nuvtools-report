# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NuvTools.Report is a .NET library suite for generating reports in multiple formats (PDF, Excel, CSV). The solution consists of three NuGet packages targeting .NET 8, .NET 9, and .NET 10:

- **src/NuvTools.Report** - Core report model infrastructure with document-table hierarchy and reflection-based data binding
- **src/NuvTools.Report.Pdf** - PDF generation using QuestPDF and PDFsharp
- **src/NuvTools.Report.Sheet** - Excel/CSV generation using ClosedXML

All libraries are published as NuGet packages. No test projects exist in the solution.

## Build and Test Commands

**Note:** This solution uses the modern `.slnx` (XML-based solution) format introduced in Visual Studio 2022 v17.11.

### Build the solution
```bash
dotnet build NuvTools.Report.slnx
```

### Build for specific configuration
```bash
dotnet build NuvTools.Report.slnx --configuration Release
```

## Architecture and Key Components

### Core Model Pattern

The library uses a **document-table-component hierarchy**:

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

### NuvTools.Report Library

Core model classes in `Table/Models/`:

- `Document` - Top-level container with `BackgroundDocumentHeaderColor` and `Tables` list
- `Table` - Individual table with `Info`, `Style`, and `Content` (Body). Contains `SetRows<T>()` for reflection-based data population
- `Info` - Metadata (Order, Name, Title, CompanyAbbreviation, CompanyUrl, LogoImage, FilterDescription, IssueDate, IssueUser)
- `Style` - Formatting (Bold, FontSize, BackgroundLineGray, BackgroundHeaderColor, FontHeaderColor)

Component classes in `Table/Models/Components/`:

- `Body` - Container for Header and Rows
- `Header` - Container for Columns
- `Column` - Column definition (Name for reflection, Label, Order, Format)
- `Row` - Data row with Order and Cells list
- `Cell` - Individual cell with Column reference and Value string

### NuvTools.Report.Pdf Library

PDF generation classes in `Table/`:

- `PdfExtension` (static) - Extension methods on `Document`:
  - `ExportSheetToPdf()` - All tables as separate base64 PDF strings
  - `ExportFirstSheetToPdf()` - First table as base64 PDF string
- `PdfSheet` (internal) - QuestPDF `IDocument` implementation with page layout (header/content/footer)
- `PdfTable` (internal) - QuestPDF `IComponent` for table content rendering
- `PdfHeader` (internal) - Header with title, filter description, logo/company info
- `PdfFooter` (internal) - Footer with issue user, date, and page numbers

Utility class in `Util/`:

- `PdfUtil` (static) - `Merge(IEnumerable<byte[]>)` for merging multiple PDFs using PDFsharp

### NuvTools.Report.Sheet Library

Extension classes in `Extensions/`:

- `ExcelExtension` (static) - `ExportToExcel()` returns base64 Excel workbook with styled headers, column definitions, and data rows
- `CsvExtensions` (static) - `ExportToCsv()` / `ExportFirstSheetToCsv()` returns base64 CSV with semicolon delimiter

### Data Population

`Table.SetRows<T>()` uses reflection to populate table content:
1. Converts Column list to Header
2. Iterates through object list
3. Uses `GetPropertyValue<T>()` to extract property values matching `Column.Name`
4. Creates Cell for each Column/property pair
5. Builds Row list from Cells

### Output Format

All export methods return **base64-encoded strings** for easy API transmission.

## Dependencies

### NuvTools.Report
- No external package dependencies

### NuvTools.Report.Pdf
- QuestPDF [2025.12.3,2026.1.0)
- PDFsharp [6.2.4,6.3.0)
- NuvTools.Report (project reference)

### NuvTools.Report.Sheet
- ClosedXML [0.105.0,0.106.0)
- NuvTools.Report (project reference)

## Code Style and Conventions

- **Nullable reference types** are enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings** are enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Code style enforcement** is enabled during build (`<EnforceCodeStyleInBuild>True</EnforceCodeStyleInBuild>`)
- **XML documentation generation** is required (`<GenerateDocumentationFile>True</GenerateDocumentationFile>`)
