# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NuvTools.Report is a .NET library suite for generating reports in multiple formats (PDF, Excel, CSV). The solution consists of three NuGet packages:

- **NuvTools.Report**: Core library providing the base table model infrastructure
- **NuvTools.Report.Pdf**: PDF generation using QuestPDF and PDFsharp
- **NuvTools.Report.Sheet**: Excel/CSV generation using ClosedXML

## Build Commands

```bash
# Build entire solution
dotnet build NuvTools.Report.slnx

# Build specific project
dotnet build src/NuvTools.Report/NuvTools.Report.csproj
dotnet build src/NuvTools.Report.Pdf/NuvTools.Report.Pdf.csproj
dotnet build src/NuvTools.Report.Sheet/NuvTools.Report.Sheet.csproj

# Build in Release mode (generates NuGet packages)
dotnet build NuvTools.Report.slnx -c Release

# Clean build artifacts
dotnet clean NuvTools.Report.slnx
```

## Target Frameworks

All projects multi-target: `net8`, `net9`, `net10.0`

Projects have:
- `Nullable` reference types enabled
- `ImplicitUsings` enabled
- XML documentation generation enabled
- Code style enforcement enabled

## Architecture

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

Key model classes in `NuvTools.Report/Table/Models/`:
- `Document.cs`: Top-level container with background color and table list
- `Table.cs`: Individual table with Info, Style, and Content (Body)
- `Info.cs`: Metadata (Order, Name, Title, Company, Logo, Filter, Issue info)
- `Style.cs`: Formatting properties
- `Components/Column.cs`: Column definition (Name, Label, Order, Format)
- `Components/Row.cs`: Row containing Cells
- `Components/Cell.cs`: Individual cell with Value and Column reference
- `Components/Body.cs`: Container for Header and Rows
- `Components/Header.cs`: Container for Columns

### Extension Pattern

Both PDF and Sheet libraries use extension methods on `Document`:

**PDF Extensions** (`NuvTools.Report.Pdf/Table/PdfExtension.cs`):
- `ExportSheetToPdf()`: Returns List<string> of base64 PDFs (one per table)
- `ExportFirstSheetToPdf()`: Returns single base64 PDF string
- Uses `PdfSheet` class internally with QuestPDF

**Sheet Extensions** (`NuvTools.Report.Sheet/Extensions/`):
- `ExcelExtension.ExportToExcel()`: Returns base64 Excel file
- `CsvExtensions.ExportToCsv()`: Returns base64 CSV file
- Uses ClosedXML for Excel generation

### Data Population

The `Table.SetRows<T>()` method uses reflection to populate table content from object lists:
1. Converts Column list to header
2. Iterates through object list
3. Uses reflection (`GetPropertyValue`) to extract property values matching Column.Name
4. Creates Cell for each Column/property pair
5. Builds Row list from Cells

## Key Dependencies

- **NuvTools.Report.Pdf**: QuestPDF 2025.7.4, PDFsharp 6.2.3
- **NuvTools.Report.Sheet**: ClosedXML 0.105.0

## Package Information

All projects:
- Generate packages on build (`GeneratePackageOnBuild`)
- Version: 10.0.0
- License: Requires acceptance (LICENSE file in repo root)
- Icon: `icon.png` in repo root
- Repository: https://github.com/nuvtools/nuvtools-report

## Important Notes

- No test projects currently exist in the solution (tests folder is defined but empty)
- All projects use the same LICENSE, README.md, and icon.png from repository root
- Strong name signing (.snk files) was recently removed from all projects
- The solution supports both Debug and Release configurations
