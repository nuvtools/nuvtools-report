using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Table.Models;

/// <summary>
/// Represents a table within a report document, containing metadata, styling, and content.
/// </summary>
public class Table
{
    /// <summary>
    /// Gets or sets the metadata information for this table, including title, company details, and issue information.
    /// </summary>
    public required Info Info { get; set; }

    /// <summary>
    /// Gets or sets the styling options for this table, such as colors and font properties.
    /// </summary>
    public Style? Style { get; set; }

    /// <summary>
    /// Gets or sets the content body of the table, including header and data rows.
    /// </summary>
    public required Body Content { get; set; }

    /// <summary>
    /// Populates the table rows from a list of objects using reflection to extract property values.
    /// </summary>
    /// <typeparam name="T">The type of objects in the list.</typeparam>
    /// <param name="columns">The column definitions that specify which properties to extract.</param>
    /// <param name="objList">The list of objects to convert into table rows.</param>
    /// <remarks>
    /// This method uses reflection to read property values from each object based on the Column.Name property.
    /// Properties that don't exist or are null will be represented as empty strings.
    /// </remarks>
    public void SetRows<T>(List<Column> columns, List<T> objList)
    {
        Content ??= new();

        if (Content.Header is null)
            Content.Header = new();

        Content.Header.Columns = columns;

        var rows = new List<Row>();

        foreach (var obj in objList)
        {
            var row = new Row { Cells = [] };

            foreach (var column in columns)
            {
                var value = GetPropertyValue(obj, column.Name);
                row.Cells.Add(new Cell
                {
                    Value = value is null ? string.Empty : value.ToString()!,
                    Column = column
                });
            }

            rows.Add(row);
        }

        Content.Rows = rows;
    }

    /// <summary>
    /// Retrieves a property value from an object using reflection.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to extract the property value from.</param>
    /// <param name="propName">The name of the property to retrieve.</param>
    /// <returns>The property value, or null if the property doesn't exist.</returns>
    private static object? GetPropertyValue<T>(T obj, string propName)
    {
        ArgumentNullException.ThrowIfNull(obj, propName);
        return obj.GetType().GetProperty(propName)?.GetValue(obj);
    }
}