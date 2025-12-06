namespace NuvTools.Report.Table.Models.Components;

/// <summary>
/// Represents a single cell in a table row, containing a value and its associated column definition.
/// </summary>
public class Cell
{
    /// <summary>
    /// Gets or sets the column definition that this cell belongs to.
    /// </summary>
    /// <remarks>
    /// The column provides metadata such as formatting, order, and display label for this cell.
    /// </remarks>
    public required Column Column { get; set; }

    /// <summary>
    /// Gets or sets the string representation of the cell's value.
    /// </summary>
    /// <remarks>
    /// All values are stored as strings. Formatting is applied based on the Column.Format property.
    /// </remarks>
    public string? Value { get; set; }
}
