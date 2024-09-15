using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Table.Models;

public class Table
{
    public required Info Info { get; set; }
    public Style? Style { get; set; }
    public required Body Content { get; set; }

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

    private static object? GetPropertyValue<T>(T obj, string propName)
    {
        ArgumentNullException.ThrowIfNull(obj, propName);
        return obj.GetType().GetProperty(propName)?.GetValue(obj);
    }
}