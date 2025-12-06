namespace NuvTools.Report.Table.Models.Components;

/// <summary>
/// Represents a single row in a table, containing a collection of cells.
/// </summary>
public class Row
{
    /// <summary>
    /// Gets or sets the display order/position of this row in the table.
    /// </summary>
    /// <remarks>
    /// Lower values appear first (higher in the table).
    /// </remarks>
    public short Order { get; set; }

    /// <summary>
    /// Gets or sets the collection of cells that make up this row.
    /// </summary>
    /// <remarks>
    /// Each cell corresponds to a column in the table and contains the actual data value.
    /// </remarks>
    public List<Cell> Cells { get; set; } = [];
}
