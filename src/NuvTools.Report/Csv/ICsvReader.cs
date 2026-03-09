namespace NuvTools.Report.Csv;

/// <summary>
/// Reads CSV content and maps it to strongly-typed objects using attribute-based field mapping.
/// </summary>
public interface ICsvReader
{
    /// <summary>
    /// Reads CSV from a string and returns a list of mapped objects.
    /// </summary>
    List<T> ReadString<T>(string content, CsvReaderOptions? options = null) where T : new();

    /// <summary>
    /// Reads CSV from a base64-encoded string and returns a list of mapped objects.
    /// </summary>
    List<T> ReadBase64<T>(string base64Content, CsvReaderOptions? options = null) where T : new();

    /// <summary>
    /// Reads CSV from a byte array and returns a list of mapped objects.
    /// </summary>
    List<T> ReadBytes<T>(byte[] data, CsvReaderOptions? options = null) where T : new();

    /// <summary>
    /// Reads CSV from a stream and returns a list of mapped objects.
    /// </summary>
    List<T> ReadStream<T>(Stream stream, CsvReaderOptions? options = null) where T : new();
}
