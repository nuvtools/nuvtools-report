namespace NuvTools.Report.Table.Models.Components;

/// <summary>
/// Represents the content body of a table, including the header definition and data rows.
/// </summary>
public class Body
{
    /// <summary>
    /// Gets or sets the header containing column definitions for the table.
    /// </summary>
    /// <remarks>
    /// The header defines the structure and metadata for all columns in the table.
    /// </remarks>
    public Header? Header { get; set; }

    /// <summary>
    /// Gets or sets the collection of data rows in the table.
    /// </summary>
    /// <remarks>
    /// Each row contains cells that correspond to the columns defined in the Header.
    /// </remarks>
    public List<Row> Rows { get; set; } = [];
}
