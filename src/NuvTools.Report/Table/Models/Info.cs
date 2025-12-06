namespace NuvTools.Report.Table.Models;

/// <summary>
/// Contains metadata and descriptive information for a table in a report.
/// </summary>
public class Info
{
    /// <summary>
    /// Gets or sets the order/position of this table when multiple tables exist in a document.
    /// </summary>
    /// <remarks>
    /// Lower values appear first. Used to determine worksheet order in Excel exports.
    /// </remarks>
    public short Order { get; set; }

    /// <summary>
    /// Gets or sets the internal name identifier for this table.
    /// </summary>
    /// <remarks>
    /// Used as the worksheet name in Excel exports.
    /// </remarks>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the display title of the table.
    /// </summary>
    /// <remarks>
    /// This title appears in the header section of exported reports.
    /// </remarks>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated name of the company generating the report.
    /// </summary>
    public string? CompanyAbbreviation { get; set; }

    /// <summary>
    /// Gets or sets the company website URL.
    /// </summary>
    /// <remarks>
    /// Displayed in the header alongside the company abbreviation.
    /// </remarks>
    public string? CompanyUrl { get; set; }

    /// <summary>
    /// Gets or sets the logo image for the report header.
    /// </summary>
    /// <remarks>
    /// Can be a file path or base64 encoded image string, depending on the export format.
    /// </remarks>
    public string? LogoImage { get; set; }

    /// <summary>
    /// Gets or sets a description of any filters or criteria applied to generate this report.
    /// </summary>
    public string? FilterDescription { get; set; }

    /// <summary>
    /// Gets or sets the date when this report was generated.
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Gets or sets the name of the user who generated this report.
    /// </summary>
    public string? IssueUser { get; set; }
}
