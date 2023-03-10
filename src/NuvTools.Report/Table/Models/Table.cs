using NuvTools.Report.Table.Models.Components;

namespace NuvTools.Report.Table.Models;

public class Table
{
    public Info Info { get; set; }
    public Style Style { get; set; }
    public Body Content { get; set; }

    public void SetRows<T>(List<Column> columns, List<T> objList)
    {
        if (Content == null)
            Content = new();

        if (Content.Header == null)
            Content.Header = new();

        Content.Header.Columns = columns;

        var rows = new List<Row>();

        foreach (var obj in objList)
        {
            var row = new Row { Cells = new List<Cell>() };

            foreach (var column in columns)
            {
                var value = GetPropertyValue(obj, column.Name);
                row.Cells.Add(new Cell
                {
                    Value = value != null ? value.ToString() : string.Empty,
                    Column = column
                });
            }

            rows.Add(row);
        }

        Content.Rows = rows;
    }

    private static object GetPropertyValue<T>(T obj, string propName)
    {
        return obj.GetType().GetProperty(propName).GetValue(obj);
    }
}