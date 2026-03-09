namespace NuvTools.Report.Sheet.FixedLength;

/// <summary>
/// Reads fixed-length (positional) content and maps it to strongly-typed objects using attribute-based field mapping.
/// </summary>
public interface IFixedLengthReader
{
    /// <summary>
    /// Reads fixed-length records from a string and returns a list of mapped objects.
    /// </summary>
    List<T> ReadString<T>(string content, FixedLengthReaderOptions? options = null) where T : new();

    /// <summary>
    /// Reads fixed-length records from a base64-encoded string and returns a list of mapped objects.
    /// </summary>
    List<T> ReadBase64<T>(string base64, FixedLengthReaderOptions? options = null) where T : new();

    /// <summary>
    /// Reads fixed-length records from a byte array and returns a list of mapped objects.
    /// </summary>
    List<T> ReadBytes<T>(byte[] data, FixedLengthReaderOptions? options = null) where T : new();

    /// <summary>
    /// Reads fixed-length records from a stream and returns a list of mapped objects.
    /// </summary>
    List<T> ReadStream<T>(Stream stream, FixedLengthReaderOptions? options = null) where T : new();
}
