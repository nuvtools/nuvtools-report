namespace NuvTools.Report.Table.Models.Components;

/// <summary>
/// Represents the header of a table, containing the column definitions.
/// </summary>
public class Header
{
    /// <summary>
    /// Gets or sets the collection of columns that define the table structure.
    /// </summary>
    /// <remarks>
    /// Each column specifies a property name, display label, order, and formatting information.
    /// </remarks>
    public List<Column> Columns { get; set; } = [];
}
