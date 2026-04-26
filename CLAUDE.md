# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NuvTools.Report is a .NET library suite for generating reports (PDF, Excel, CSV) and reading structured input (CSV, fixed-length/positional). It ships **four** NuGet packages multi-targeting net8/net9/net10:

- **src/NuvTools.Report** — Core models *and* abstractions (interfaces, attributes, options, converters) for every implementation package. No external runtime dependencies beyond `Microsoft.Extensions.DependencyInjection.Abstractions` consumers see indirectly.
- **src/NuvTools.Report.Pdf** — PDF implementation using QuestPDF + PDFsharp.
- **src/NuvTools.Report.Sheet** — CSV + Excel implementation using ClosedXML.
- **src/NuvTools.Report.FixedLength** — Fixed-length (positional) text reader. No ClosedXML dependency.

Tests live under `tests/` (NUnit 4.x, **net10.0 only**).

## Build and Test Commands

The solution uses the `.slnx` (XML-based) format (VS 2022 v17.11+).

```bash
# Build everything
dotnet build NuvTools.Report.slnx

# Release build (also produces NuGet packages — GeneratePackageOnBuild is on)
dotnet build NuvTools.Report.slnx -c Release

# Run all tests
dotnet test NuvTools.Report.slnx

# Run tests for a single project
dotnet test tests/NuvTools.Report.Sheet.Tests/NuvTools.Report.Sheet.Tests.csproj

# Filter by test name (NUnit fully-qualified)
dotnet test --filter "FullyQualifiedName~CsvExporterTests"
```

## Architecture

### Abstraction / implementation split

The core `NuvTools.Report` package owns **all** interfaces, attributes, options, models, and converter contracts so the application layer (clean-architecture style) only references the abstraction package; the composition root injects concrete implementations. Each implementation package consumes those contracts.

When adding a new feature, the contract usually belongs in `NuvTools.Report` and the implementation in `NuvTools.Report.Pdf`, `NuvTools.Report.Sheet`, or `NuvTools.Report.FixedLength`.

```
NuvTools.Report                          Implementation packages
─────────────────────────                ─────────────────────────────
Pdf/IPdfExporter, IPdfMerger      ←──    NuvTools.Report.Pdf
                                          (Pdf.Table.PdfExporter, Pdf.Util.PdfMerger)
Sheet/Csv/ICsvReader, ICsvExporter ←──   NuvTools.Report.Sheet
Sheet/Excel/IExcelExporter         ←──   (Sheet.Csv.{CsvReader,CsvExporter},
                                           Sheet.Excel.ExcelExporter)
FixedLength/IFixedLengthReader     ←──   NuvTools.Report.FixedLength
                                          (FixedLength.FixedLengthReader)
Parsing/{IFieldConverter, FieldConverter<T>,
  BuiltInConverters, TrimMode,
  ParseException}                          shared by Csv and FixedLength readers
Sheet/Csv/Attributes
FixedLength/Attributes
Table/Models (Document, Table, Info,
  Style, Body, Header, Column, Row, Cell)
```

`NuvTools.Report` declares `[InternalsVisibleTo("NuvTools.Report.Sheet")]` so the Sheet implementation can use internal helpers (e.g., `BuiltInConverters`) without exposing them publicly. The same will apply to `NuvTools.Report.FixedLength` if it ever needs internal access — currently it only uses public types.

**Note:** FixedLength is *not* under the `Sheet` namespace — fixed-length files are flat positional records (banking, EDI, mainframe) unrelated to spreadsheet rendering. Shared parsing types live at `NuvTools.Report.Parsing` (no `Sheet` prefix) precisely because both Csv and FixedLength readers consume them.

### DI registration

Each implementation package exposes a `ServiceCollectionExtensions` registering the public interfaces as singletons:

- `AddPdfReportServices()` → `IPdfExporter`, `IPdfMerger`
- `AddSheetReportServices()` → `ICsvReader`, `ICsvExporter`, `IExcelExporter`
- `AddFixedLengthReportServices()` → `IFixedLengthReader`

Implementations are stateless — keep them that way so the singleton registration stays valid.

### Document / table model (export pipeline)

Hierarchy used by the PDF and Excel exporters:

```
Document
└── Tables (List<Table>)
    ├── Info       (title, company, logo, issue date/user, filter description)
    ├── Style      (header colors, font, bold, line gray)
    └── Content (Body)
        ├── Header → List<Column>   (Name = property to bind, Label, Order, Format)
        └── Rows   → List<Row> → List<Cell>   (each Cell references a Column + Value)
```

`Table.SetRows<T>(columns, items)` is the reflection-based binder: it copies the column list into `Body.Header`, then for each item in `items` it reads `Column.Name` as a property name on `T` and emits one `Cell` per column. Format strings on `Column` are applied during rendering, not during binding.

Output of the PDF/Excel/CSV exporters is always **base64-encoded** for direct API transmission. `ICsvExporter.ExportToCsv` returns one base64 string per table; `ExportFirstSheetToCsv` returns a single string.

### Attribute-driven import pipeline

`ICsvReader` (in `NuvTools.Report.Sheet`) and `IFixedLengthReader` (in `NuvTools.Report.FixedLength`) map text input to `T` using:

- `[CsvRecord]` / `[FixedLengthRecord]` on the class (record-level options like delimiter or `AllowShorterLines`).
- `[CsvField(index, ...)]` / `[FixedLengthField(index, length, ...)]` on each property (position, format, trim, optional, custom converter).

Type conversion goes through `Parsing/Converters` in the base `NuvTools.Report` package:

- `BuiltInConverters` covers string/int/long/short/decimal/double/float/bool/DateTime/DateOnly/Guid/enums and their nullable variants (numerics use `InvariantCulture`; bool accepts `true|false|1|0`). It is `internal` to `NuvTools.Report` and exposed to implementation packages via `InternalsVisibleTo`.
- For anything custom, implement `IFieldConverter` (untyped) or extend `FieldConverter<T>` (typed) and reference the converter type via `Converter = typeof(...)` on the field attribute. The `Format` attribute property is forwarded to the converter.

When extending parsing, add the converter under `Parsing/Converters` in **NuvTools.Report** (not the implementation packages) so both readers can use it.

### CSV specifics

- Default delimiter is **comma** (RFC 4180); `CsvDelimiter` enum also covers `Semicolon`, `Tab`, `Pipe`, `Custom`.
- `ICsvExporter` defaults to including a header row and **sanitizing** delimiter occurrences from values to prevent corruption. Both are toggleable per call (`includeHeader`, `sanitizeDelimiter`).
- `CsvReaderOptions` can override the attribute-declared delimiter at runtime, plus `SkipHeader`, `IgnoreEmptyLines`, `HandleQuotedFields`.
- `CsvFieldExtensions` exposes `GetFieldCaptions()` and `GetCsvHeader(delimiter)` for emitting header lines from a record type.

### PDF specifics

`PdfExporter` builds a QuestPDF `IDocument` per table (`PdfSheet`) composed of `PdfHeader` / `PdfTable` / `PdfFooter`. `PdfMerger.Merge(IEnumerable<byte[]>)` uses PDFsharp to concatenate already-rendered PDFs — use it when callers need a single file from multi-table output.

## Conventions

- Nullable reference types **on**; implicit usings **on**; `EnforceCodeStyleInBuild` **on**; XML doc generation required (public APIs must be documented).
- Library projects target `net8;net9;net10.0`. Test projects target **net10.0 only** — do not multi-target test code.
- Public types belong in `NuvTools.Report` (the abstraction package) unless they are inherently tied to QuestPDF/PDFsharp/ClosedXML.
- New DI-registrable services should be added to the matching `ServiceCollectionExtensions` and registered as singletons (state-free).
