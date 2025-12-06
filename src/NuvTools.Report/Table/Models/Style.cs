namespace NuvTools.Report.Table.Models;

/// <summary>
/// Defines styling and formatting options for table elements.
/// </summary>
public class Style
{
    /// <summary>
    /// Gets or sets whether text should be rendered in bold.
    /// </summary>
    public bool? Bold { get; set; }

    /// <summary>
    /// Gets or sets the font size in points.
    /// </summary>
    public double? FontSize { get; set; }

    /// <summary>
    /// Gets or sets whether to apply a light gray background to rows.
    /// </summary>
    /// <remarks>
    /// Typically used for alternating row colors or header rows.
    /// </remarks>
    public bool? BackgroundLineGray { get; set; }

    /// <summary>
    /// Gets or sets the background color for header elements.
    /// </summary>
    /// <remarks>
    /// Color should be in HTML color format (e.g., "#FFFFFF" or "white").
    /// </remarks>
    public string BackgroundHeaderColor { get; set; } = "#FFFFFF";

    /// <summary>
    /// Gets or sets the font color for header text.
    /// </summary>
    /// <remarks>
    /// Color should be in HTML color format (e.g., "#000000" or "black").
    /// </remarks>
    public string FontHeaderColor { get; set; } = "#000000";
}