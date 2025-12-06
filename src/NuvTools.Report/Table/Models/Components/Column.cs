namespace NuvTools.Report.Table.Models.Components;

/// <summary>
/// Represents a column definition in a table, specifying how data should be displayed.
/// </summary>
public class Column
{
    /// <summary>
    /// Gets or sets the property name to extract from source objects.
    /// </summary>
    /// <remarks>
    /// This name is used with reflection in Table.SetRows to read property values from objects.
    /// Must match the exact property name on the source object (case-sensitive).
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the display label shown in the column header.
    /// </summary>
    /// <remarks>
    /// This is the user-friendly text displayed in the table header, which can differ from the property Name.
    /// </remarks>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the display order/position of this column in the table.
    /// </summary>
    /// <remarks>
    /// Lower values appear first (leftmost). Columns are sorted by this value when rendered.
    /// </remarks>
    public short Order { get; set; }

    /// <summary>
    /// Gets or sets the format string for displaying values in this column.
    /// </summary>
    /// <remarks>
    /// Used for formatting dates, numbers, and other data types. Examples: "yyyy-MM-dd", "N2", "C".
    /// </remarks>
    public string? Format { get; set; }
}
