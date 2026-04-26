namespace NuvTools.Report.Parsing;

/// <summary>
/// Specifies how whitespace should be trimmed from field values.
/// </summary>
public enum TrimMode
{
    /// <summary>No trimming.</summary>
    None,
    /// <summary>Trim leading whitespace.</summary>
    Left,
    /// <summary>Trim trailing whitespace.</summary>
    Right,
    /// <summary>Trim both leading and trailing whitespace.</summary>
    Both
}
