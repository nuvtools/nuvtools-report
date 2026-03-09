namespace NuvTools.Report.Sheet.FixedLength.Attributes;

/// <summary>
/// Specifies fixed-length record-level settings.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class FixedLengthRecordAttribute : Attribute
{
    /// <summary>
    /// Gets or sets whether lines shorter than the total expected length are allowed. Defaults to <c>false</c>.
    /// </summary>
    public bool AllowShorterLines { get; set; }
}
